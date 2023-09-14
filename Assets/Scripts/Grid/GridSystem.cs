using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem : SingletonBase<GridSystem>
{
    [SerializeField] private int _width = 5;
    [SerializeField] private int _length = 5;
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private GridCell _gridCellPrefab;

    public GridCell GetCell(Vector3 worldPosition)
    {
        var cellCoordinates = GetCellCoordinates(worldPosition);
        
        return IsCellValid(cellCoordinates) ? _gridCells[cellCoordinates.x, cellCoordinates.y] : null;
    }

    public GridCell GetCell(Vector2Int cellCoordinates)
    {
        return GetCell(cellCoordinates.x, cellCoordinates.y);
    }
    
    public GridCell GetCell(int cellX, int cellY)
    {
        return _gridCells[cellX, cellY];
    }
    
    public Vector2Int GetCellCoordinates(Vector3 worldPosition)
    {
        var relativePos = worldPosition - _transform.position;
        
        var roughX = Mathf.RoundToInt(relativePos.x / _cellSize);
        var roughY = Mathf.RoundToInt(relativePos.z / _cellSize / HEX_VERTICAL_POSITION_MULTIPLIER);
    
        var roughCellCoordinates = new Vector2Int(roughX, roughY);

        var neighbourCells = GetNeighbourCellCoordinates(roughCellCoordinates);
    
        var closestCell = roughCellCoordinates;
    
        foreach (var neighbourCell in neighbourCells)
        {
            var currentClosestDistance = Vector3.Distance(worldPosition, GetWorldPosition(closestCell)); 
            var newDistance = Vector3.Distance(worldPosition, GetWorldPosition(neighbourCell));
            
            if (newDistance < currentClosestDistance) {
                // Closer than closest
                closestCell = neighbourCell;
            }
    
        }

        return closestCell;
    }

    public HashSet<GridCell> CollectCellsAroundCell(GridCell pivotCell, int distance)
    {
        var cells = new HashSet<GridCell>();
        CollectCellsAroundCellInternal(pivotCell, distance, cells, new HashSet<GridCell>());
        return cells;
    }

    public void MarkAllCellsDeactivated()
    {
        foreach (var cell in _gridCells)
        {
            cell.SetActive(false);
        }
    }
    
    public void MarkAllCellsActive()
    {
        foreach (var cell in _gridCells)
        {
            cell.SetActive(true);
        }
    }

    private const float HEX_VERTICAL_POSITION_MULTIPLIER = 0.75f;

    private Transform _transform;
    private GridCell[,] _gridCells;
    

    protected override void Awake()
    {
        base.Awake();

        _transform = transform;
        BuildGrid();
    }

    private void BuildGrid()
    {
        _gridCells = new GridCell[_width, _length];
        
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _length; j++)
            {
                var cellLocalPos = GetWorldPosition(i, j);
                _gridCells[i, j] = Instantiate(_gridCellPrefab, cellLocalPos, Quaternion.identity, _transform);
                _gridCells[i, j].Coordinates = new Vector2Int(i, j);
            }
        }
    }

    private Vector3 GetWorldPosition(Vector2Int cellCoordinates)
    {
        return GetWorldPosition(cellCoordinates.x, cellCoordinates.y);
    }
    
    private Vector3 GetWorldPosition(int cellX, int cellY)
    {
        var isOddRow = cellY % 2 != 0;
        
        var x = cellX * _cellSize;
        if (isOddRow)
        {
            //odd hexRows are shifted on half of cell
            x += 0.5f * _cellSize;
        }

        var z = cellY * _cellSize * HEX_VERTICAL_POSITION_MULTIPLIER;

        var originPosition = _transform.position;

        return new Vector3(x, 0, z) + originPosition;
    }
    
    private List<Vector2Int> GetNeighbourCellCoordinates(Vector2Int cellCoordinates)
    {
        var isOddRow = cellCoordinates.y % 2 != 0;
    
        var neighbourCells = new List<Vector2Int> {
            cellCoordinates + new Vector2Int(-1, 0),
            cellCoordinates + new Vector2Int(+1, 0),
    
            cellCoordinates + new Vector2Int(isOddRow ? +1 : -1, +1),
            cellCoordinates + new Vector2Int(+0, +1),
    
            cellCoordinates + new Vector2Int(isOddRow ? +1 : -1, -1),
            cellCoordinates + new Vector2Int(+0, -1),
        };

        return neighbourCells;
    }
    
    private List<GridCell> GetCellNeighbours(GridCell cell)
    {
        var neighbourCellsCoordinates = GetNeighbourCellCoordinates(cell.Coordinates);

        var neighbourCells = new List<GridCell>();
        
        foreach (var coordinates in neighbourCellsCoordinates)
        {
            if (IsCellValid(coordinates))
            {
                neighbourCells.Add(GetCell(coordinates));
            }
        }

        return neighbourCells;
    }

    private void CollectCellsAroundCellInternal(GridCell pivotCell, int distance, HashSet<GridCell> collectedCells, HashSet<GridCell> processedCells)
    {
        processedCells.Add(pivotCell);
        distance--;
        foreach (var cell in GetCellNeighbours(pivotCell))
        {
            collectedCells.Add(cell);
            if (distance > 0 && !processedCells.Contains(cell))
            {
                CollectCellsAroundCellInternal(cell, distance, collectedCells, processedCells);
            }
        }
    }

    private bool IsCellValid(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x < _width && coordinates.y < _length;
    }
}
