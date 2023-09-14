using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : SingletonBase<BattleController>
{
    public event Action<BattleState> OnStateChanged;
    public event Action<Players> OnGameOver;
    public event Action<UnitStackController> OnUnitRegistered;

    [SerializeField] private List<UnitStackInitialData> _playerOneUnitDatas;
    [SerializeField] private List<UnitStackInitialData> _playerTwoUnitDatas;

    [SerializeField] private UnitStackController _unitPrefab;

    public void EndTurn()
    {
        _activeUnit.SetSelected(false);

        if (_currentTurnQueue.Count == 0)
        {
            RefillTurnQueue();
        }
        
        _activeUnit = _currentTurnQueue.Dequeue();
        _activeUnit.SetSelected(true);

        SetState(BattleState.MOVEMENT);
    }

    public void RegisterUnit(UnitStackController unitToRegister)
    {
        _onDieHandlers.Add(unitToRegister, () => UnregisterUnit(unitToRegister));
        unitToRegister.OnDie += _onDieHandlers[unitToRegister];
        
        for (var i = 0; i < _turnOrder.Count; i++)
        {
            var unit = _turnOrder[i];

            if (unit.GetInitiative() >= unitToRegister.GetInitiative()) continue;
            
            _turnOrder.Insert(i, unitToRegister);
            return;
        }
        
        _turnOrder.Add(unitToRegister);
        
        OnUnitRegistered?.Invoke(unitToRegister);
    }
    
    public void UnregisterUnit(UnitStackController unitToUnregister)
    {
        _turnOrder.Remove(unitToUnregister);
        
        var newTurnQueue = new Queue<UnitStackController>();
        foreach (var unit in _currentTurnQueue)
        {
            if (unit == unitToUnregister) continue;
            
            newTurnQueue.Enqueue(unit);
        }
        
        _currentTurnQueue.Clear();
        _currentTurnQueue = newTurnQueue;

        CheckForWin();
    }

    public void TrySetMoveState()
    {
        SetState(BattleState.MOVEMENT);
    }
    
    public void TrySetAttackState()
    {
        SetState(BattleState.ATTACK);
    }

    public BattleState GetCurrentState()
    {
        return _currentState;
    }
    
    private GridSystem _gridSystem;
    private CursorManager _cursorManager;

    private UnitStackController _activeUnit;
    private List<UnitStackController> _turnOrder = new();
    private Queue<UnitStackController> _currentTurnQueue = new();
    
    private Dictionary<UnitStackController, Action> _onDieHandlers = new();

    private BattleState _currentState = BattleState.MOVEMENT;

    private void Start()
    {
        _gridSystem = GridSystem.Instance;

        _cursorManager = CursorManager.Instance;
        _cursorManager.OnClick += HandleOnClick;
        
        SpawnUnits(_playerOneUnitDatas, Players.PLAYER_ONE);
        SpawnUnits(_playerTwoUnitDatas, Players.PLAYER_TWO);

        RefillTurnQueue();
        _activeUnit = _currentTurnQueue.Dequeue();
        _activeUnit.SetSelected(true);
        
        
        SetState(BattleState.MOVEMENT);
    }

    private void OnDestroy()
    {
        _cursorManager.OnClick -= HandleOnClick;

        foreach (var unit in _turnOrder)
        {
            if (!unit) return;

            unit.OnDie -= _onDieHandlers[unit];
            _onDieHandlers.Remove(unit);
        }
    }

    private void SpawnUnits(List<UnitStackInitialData> unitDatas, Players side)
    {
        foreach (var unitInitData in unitDatas)
        {
            var cell = _gridSystem.GetCell(unitInitData.InitialPosition);

            var unit = Instantiate(_unitPrefab, cell.transform.position, Quaternion.identity);
            unit.Initialize(unitInitData.InitialCount, unitInitData.UnitData, cell);
            unit.SetSide(side);
            
            RegisterUnit(unit);
        }
    }

    private void HandleOnClick(GridCell cell)
    {
        switch (_currentState)
        {
            case BattleState.MOVEMENT:
                _activeUnit.TryMoveTo(cell);
                SetState(BattleState.ATTACK);
                break;
            case BattleState.ATTACK:
                _activeUnit.TryAttack(cell.GetUnit());
                SetState(BattleState.WAITING);
                break;
            case BattleState.GAME_OVER:
                break;
            case BattleState.WAITING:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
    
    private void RefillTurnQueue()
    {
        foreach (var unit in _turnOrder)
        {
            _currentTurnQueue.Enqueue(unit);
        }
    }

    private void CheckForWin()
    {
        var thereIsNoPlayerOneUnits = true;
        var thereIsNoPlayerTwoUnits = true;
        
        foreach (var unit in _turnOrder)
        {
            if (unit.GetSide() == Players.PLAYER_ONE)
            {
                thereIsNoPlayerOneUnits = false;
            }
            else
            {
                thereIsNoPlayerTwoUnits = false;
            }
        }
        
        if (!thereIsNoPlayerOneUnits && !thereIsNoPlayerTwoUnits) return;

        SetState(BattleState.GAME_OVER);
        
        var winner = thereIsNoPlayerOneUnits ? Players.PLAYER_TWO : Players.PLAYER_ONE;
        OnGameOver?.Invoke(winner);
    }

    private void SetState(BattleState newState)
    {
        _currentState = newState;
        OnStateChanged?.Invoke(_currentState);

        switch (newState)
        {
            case BattleState.MOVEMENT:
                _activeUnit.MarkCellsForMove();
                break;
            case BattleState.ATTACK:
                _activeUnit.MarkCellsForAttack();
                break;
            case BattleState.WAITING:
                _gridSystem.MarkAllCellsActive();
                break;
            case BattleState.GAME_OVER:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    [Serializable]
    private class UnitStackInitialData
    {
        [SerializeField] public UnitData UnitData;
        [SerializeField] public int InitialCount = 1;
        [SerializeField] public Vector2Int InitialPosition = Vector2Int.zero;
    }
}

public enum BattleState
{
    MOVEMENT,
    ATTACK,
    WAITING,
    GAME_OVER
}

public enum Players
{
    PLAYER_ONE,
    PLAYER_TWO
}