using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _attackButton;
    
    [SerializeField] private TMP_Text _gameOverLabel;
    [SerializeField] private TMP_Text _battleLog;

    private const string PLAYER_ONE_WIN_TEXT = "Player One wins!";
    private const string PLAYER_TWO_WIN_TEXT = "Player Two wins!";

    private BattleLogSystem _battleLogSystem = new BattleLogSystem();
    
    private BattleController _battleController;

    private void Start()
    {
        _battleController = BattleController.Instance;
        _battleController.OnStateChanged += HandleBattleStateChanged;
        _battleController.OnGameOver += HandleGameOver;
        HandleBattleStateChanged(_battleController.GetCurrentState());
        
        
        _endTurnButton.onClick.AddListener(EndTurn);
        _moveButton.onClick.AddListener(SetMoveMode);
        _attackButton.onClick.AddListener(SetAttackMode);
    }

    private void OnDestroy()
    {
        _battleController.OnStateChanged -= HandleBattleStateChanged;
        _battleController.OnGameOver -= HandleGameOver;
        
        _endTurnButton.onClick.RemoveListener(EndTurn);
        _moveButton.onClick.RemoveListener(SetMoveMode);
        _attackButton.onClick.RemoveListener(SetAttackMode);
    }

    private void HandleBattleStateChanged(BattleState _newState)
    {
        switch (_newState)
        {
            case BattleState.MOVEMENT:
                _moveButton.Select();
                break;
            case BattleState.ATTACK:
                _attackButton.Select();
                break;
            case BattleState.WAITING:
                _endTurnButton.Select();
                break;
            case BattleState.GAME_OVER:
                _endTurnButton.gameObject.SetActive(false);
                _attackButton.gameObject.SetActive(false);
                _moveButton.gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_newState), _newState, null);
        }
    }

    private void HandleGameOver(Players winner)
    {
        var gameOverText = winner switch
        {
            Players.PLAYER_ONE => PLAYER_ONE_WIN_TEXT,
            Players.PLAYER_TWO => PLAYER_TWO_WIN_TEXT,
            _ => throw new ArgumentOutOfRangeException(nameof(winner), winner, null)
        };

        _gameOverLabel.text = gameOverText;
        _gameOverLabel.gameObject.SetActive(true);
    }

    private void EndTurn()
    {
        _battleController.EndTurn();
    }

    private void SetMoveMode()
    {
        _battleController.TrySetMoveState();
    }

    private void SetAttackMode()
    {
        _battleController.TrySetAttackState();
    }
}
