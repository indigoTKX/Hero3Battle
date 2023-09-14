using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit", fileName = "NewUnit")]
public class UnitData : ScriptableObject
{
    [SerializeField] public string Name = "Unit";
    [SerializeField] public int Health = 10;
    [SerializeField] public int Movement = 5;
    [SerializeField] public int MinDamage = 5;
    [SerializeField] public int MaxDamage = 6;
    [SerializeField] public int Initiative = 5;
    [SerializeField] public int AttackRange = 1;
}
