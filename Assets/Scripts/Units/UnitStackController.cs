using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitStackController : MonoBehaviour
{
    public event Action<int, UnitData> OnInitialized; 
    public event Action<bool> OnSelected; 
    public event Action<GridCell> OnMoved;
    public event Action<UnitStackController> OnAttack;
    public event Action OnDamaged;
    public event Action OnDie;

    public void Initialize(int initialStackCount, UnitData unitData, GridCell cell)
    {
        _gridSystem = GridSystem.Instance;
        
        _occupiedCell = cell;
        _occupiedCell.SetUnit(this);
        
        _health = unitData.Health;
        _movementDistance = unitData.Movement;
        _minDamage = unitData.MinDamage;
        _maxDamage = unitData.MaxDamage;
        _initiative = unitData.Initiative;
        _attackRange = unitData.AttackRange;
        _name = unitData.Name;
        
        _currentStackCount = initialStackCount;
        _currentTopUnitHealth = _health;
        
        OnInitialized?.Invoke(initialStackCount, unitData);
    }

    public void SetSide(Players side)
    {
        _side = side;

        var unitForwardDirection = _side == Players.PLAYER_ONE ? Vector3.right : Vector3.left;
        transform.rotation = Quaternion.LookRotation(unitForwardDirection, Vector3.up);
    }

    public void SetSelected(bool isSelected)
    {
        _isMovedThisTurn = false;
        _isAttackedThisTurn = false;
        _isSelected = isSelected;
        
        OnSelected?.Invoke(_isSelected);
    }

    public void MarkCellsForMove()
    {
        _gridSystem.MarkAllCellsDeactivated();
        var cellsInRange = _gridSystem.CollectCellsAroundCell(_occupiedCell, _movementDistance);
        foreach (var cell in cellsInRange)
        {
            if (!cell.IsEmpty()) continue;
            
            cell.SetActive(true);
        }
    }
    
    public void MarkCellsForAttack()
    {
        _gridSystem.MarkAllCellsDeactivated();
        var cellsInRange = _gridSystem.CollectCellsAroundCell(_occupiedCell, _attackRange);
        foreach (var cell in cellsInRange)
        {
            if (cell.IsEmpty() || cell.GetUnit().GetSide() == _side) continue;
            
            cell.SetActive(true);
        }
    }

    public void TryMoveTo(GridCell newCell)
    {
        if (_isMovedThisTurn) return;
        
        _occupiedCell.SetUnit(null);

        transform.position = newCell.transform.position;
        _occupiedCell = newCell;
        _occupiedCell.SetUnit(this);

        _isMovedThisTurn = true;
        OnMoved?.Invoke(newCell);
    }

    public void TryAttack(UnitStackController unitToAttack)
    {
        if (_isAttackedThisTurn) return;

        var damage = Random.Range(_minDamage * _currentStackCount, _maxDamage * _currentStackCount);
        unitToAttack.DealDamage(damage);
        
        _isAttackedThisTurn = true;
        OnAttack?.Invoke(unitToAttack);
    }

    public void DealDamage(int damage)
    {
        var fullHealth = _health * (_currentStackCount - 1) + _currentTopUnitHealth;
        fullHealth -= damage;

        _currentStackCount = fullHealth / _health + 1;
        _currentTopUnitHealth = fullHealth % _health;
        if (_currentTopUnitHealth == 0)
        {
            _currentTopUnitHealth = _health;
            _currentStackCount -= 1;
        }

        OnDamaged?.Invoke();

        if (fullHealth > 0) return;
        
        OnDie?.Invoke();
        Destroy(gameObject);
    }

    public int GetInitiative()
    {
        return _initiative;
    }

    public int GetCurrentStackCount()
    {
        return _currentStackCount;
    }

    public Players GetSide()
    {
        return _side;
    }

    public string GetName()
    {
        return name;
    }

    private GridSystem _gridSystem;
    
    private int _health;
    private int _movementDistance;
    private int _minDamage;
    private int _maxDamage;
    private int _initiative;
    private int _attackRange;
    private string _name;
    
    private int _currentStackCount;
    private int _currentTopUnitHealth;
    
    private Players _side;
    private GridCell _occupiedCell;

    private bool _isSelected;
    private bool _isMovedThisTurn;
    private bool _isAttackedThisTurn;
}
