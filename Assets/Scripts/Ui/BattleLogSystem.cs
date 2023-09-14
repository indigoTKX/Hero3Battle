using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleLogSystem
{
    public void Initialize(TMP_Text battleLogTextfield, BattleController battleController)
    {
        _battleLog = battleLogTextfield;
        _battleController = battleController;
        
        // _battleController.OnUnitRegistered += 
    }
    
    private TMP_Text _battleLog;
    private BattleController _battleController;
    private Dictionary<UnitStackController, Action<UnitStackController>> _onAttackHandlers = new();

    private void SubscribeOnUnit(UnitStackController unit)
    {
        _onAttackHandlers.Add(unit, (targetUnit) => LogAttack(unit, targetUnit));
        unit.OnAttack += _onAttackHandlers[unit];
    }

    private void LogAttack(UnitStackController attackingUnit, UnitStackController attackedUnit)
    {
        var attackingUnitName = attackingUnit.GetName();
        var attackedUnitName = attackedUnit.GetName();
        // var logText = $"{attackingUnit} attacked {attackedUnit} for {}";
    }
}
