using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private GameObject _selectedOutline;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _deactivatedMaterial;
    
    public Vector2Int Coordinates { get; set; }

    public void SetUnit(UnitStackController newUnit)
    {
        _unit = newUnit;
        if (!_unit) return;
    }

    public UnitStackController GetUnit()
    {
        return _unit;
    }

    public bool IsEmpty()
    {
        return _unit == null;
    }
    
    public void SetHovered(bool isHovered)
    {
        _selectedOutline.SetActive(isHovered);
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;

        _renderer.material = _isActive ? _activeMaterial : _deactivatedMaterial;
    }

    public bool IsActive()
    {
        return _isActive;
    }

    private UnitStackController _unit = null;

    private Material _activeMaterial;
    private bool _isActive;

    private void Awake()
    {
        _activeMaterial = _renderer.material;
    }
}
