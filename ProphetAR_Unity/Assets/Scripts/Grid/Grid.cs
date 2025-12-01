using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    [ExecuteAlways]
    public partial class Grid : MonoBehaviour
    {
        [Tooltip("(rows, columns)")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _gridDimensions = default;

        [SerializeField]
        [ReadOnly]
        private Transform _cellsParent = null;

        [SerializeField]
        private Vector2 _cellDimensions = default;

        [SerializeField]
        private GridCell _originCell = null;

        [SerializeField]
        private List<GridCell> _serializedCells = null;

        public Transform CellsParent => _cellsParent;
        
        public Vector2 GridDimensions => _gridDimensions;

        public Vector2 CellDimensions => _cellDimensions;

        public Vector2 MinCoordinates { get; private set; }
        
        public Vector2 MaxCoordinates { get; private set; }
        
        public GridCell OriginCell => _originCell;
        
        private readonly Dictionary<Vector2, GridCell> _cells = new();

        private void Awake()
        {
            RebuildGrid();
        }

        private void RecalculateMinMaxCoordinates()
        { 
            bool hasMinX = false; 
            bool hasMinY = false;
            
            bool hasMaxX = false; 
            bool hasMaxY = false;
            
            foreach (GridCell serializedCell in _serializedCells)
            {
                if (!hasMinX || serializedCell.Coordinates.x < MinCoordinates.x)
                {
                    hasMinX = true;
                    MinCoordinates = MinCoordinates.WithX(serializedCell.Coordinates.x);
                }

                if (!hasMinY || serializedCell.Coordinates.y < MinCoordinates.y)
                {
                    hasMinY = true;
                    MinCoordinates = MinCoordinates.WithY(serializedCell.Coordinates.y);
                }

                if (!hasMaxX || serializedCell.Coordinates.x > MaxCoordinates.x)
                {
                    hasMaxX = true;
                    MaxCoordinates = MaxCoordinates.WithX(serializedCell.Coordinates.x);
                }

                if (!hasMaxY || serializedCell.Coordinates.y > MaxCoordinates.y)
                {
                    hasMaxY = true;
                    MaxCoordinates = MaxCoordinates.WithY(serializedCell.Coordinates.y);
                }
            }
            
            _gridDimensions = new Vector2(MaxCoordinates.x - MinCoordinates.x + 1, MaxCoordinates.y - MinCoordinates.y + 1);
        }

        private void RecalculateCellNeighbours(GridCell cell)
        {
            if (_cells.TryGetValue(cell.Coordinates + Vector2.right, out GridCell right))
            {
                cell.SetRightCell(right);
            }
            
            if (_cells.TryGetValue(cell.Coordinates - Vector2.right, out GridCell left))
            {
                cell.SetLeftCell(left);
            }

            if (_cells.TryGetValue(cell.Coordinates + Vector2.up, out GridCell down))
            {
                cell.SetDownCell(down);
            }

            if (_cells.TryGetValue(cell.Coordinates - Vector2.up, out GridCell up))
            {
                cell.SetUpCell(up);
            }
        }

        public void RebuildGrid()
        {
            _cells.Clear();
            
            foreach (GridCell serializedCell in _serializedCells)
            {
                serializedCell.RecalculateCoordinates();
                serializedCell.name = $"Cell ({serializedCell.Coordinates.x}, {serializedCell.Coordinates.y})";
                _cells.Add(serializedCell.Coordinates, serializedCell);
            }
            
            foreach (GridCell cell in _cells.Values)
            {
                RecalculateCellNeighbours(cell);
            }
            
            RecalculateMinMaxCoordinates();
        }
    }
}