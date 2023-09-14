using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : SingletonBase<CursorManager>
{
    public event Action<GridCell> OnClick; 

    [SerializeField] private LayerMask _layersForCursor;
    [SerializeField] private bool _dbgDrawCursorProjection = true;
    [SerializeField] private Transform _cursorProjectionPrefab;

    public Vector3 GetCursorWorldPosition()
    {
        var cursorScreenPosition = Input.mousePosition;
        var rayToCursor = _camera.ScreenPointToRay(cursorScreenPosition);

        var cursorWorldPosition = Vector3.positiveInfinity;
        if (Physics.Raycast(rayToCursor, out var hit, float.MaxValue, _layersForCursor))
        {
            cursorWorldPosition = hit.point;
            if (_dbgDrawCursorProjection)
            {
                _cursorProjection.position = cursorWorldPosition;
            } 
        };
        return cursorWorldPosition;
    }
    
    private Camera _camera;
    private Transform _cursorProjection;
    private GridSystem _gridSystem;
    private GridCell _currentHoveredCell = null;
    
    protected override void Awake()
    {
        base.Awake();
        
        _camera = Camera.main;

        if(!_dbgDrawCursorProjection) return;
        _cursorProjection = Instantiate(_cursorProjectionPrefab);
    }

    private void Start()
    {
        _gridSystem = GridSystem.Instance;
    }

    private void Update()
    {
        if (_currentHoveredCell)
        {
            _currentHoveredCell.SetHovered(false);
        }
        
        _currentHoveredCell = _gridSystem.GetCell(GetCursorWorldPosition());
        if (!_currentHoveredCell  || !_currentHoveredCell.IsActive()) return;
        
        _currentHoveredCell.SetHovered(true);

        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        
        OnClick?.Invoke(_currentHoveredCell);
    }
}
