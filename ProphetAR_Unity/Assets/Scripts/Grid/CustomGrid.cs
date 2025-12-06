using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public partial class CustomGrid : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Tooltip("(rows, columns)")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _gridDimensions = default;

        [SerializeField]
        [ReadOnly]
        private Vector2 _minCoordinate = default;
        
        [SerializeField]
        [ReadOnly]
        private Vector2 _maxCoordinate = default;
        
        [SerializeField]
        private GridSection _originGridSection = null;
        
        [SerializeField]
        [ReadOnly]
        private List<SavedGridCell> _savedGrid = null;
        
        private readonly Dictionary<Vector2, GridCell> _grid = new();

        private void RecalculateCellNeighbours(GridCell cell)
        {
            if (_grid.TryGetValue(cell.Coordinates + Vector2.right, out GridCell right))
            {
                cell.SetRightCell(right);
            }
            
            if (_grid.TryGetValue(cell.Coordinates - Vector2.right, out GridCell left))
            {
                cell.SetLeftCell(left);
            }

            if (_grid.TryGetValue(cell.Coordinates + Vector2.up, out GridCell down))
            {
                cell.SetDownCell(down);
            }

            if (_grid.TryGetValue(cell.Coordinates - Vector2.up, out GridCell up))
            {
                cell.SetUpCell(up);
            }
        }

        public void BuildGrid()
        {
            _grid.Clear();

            foreach (SavedGridCell savedGridCell in _savedGrid)
            {
                _grid[savedGridCell.Coordinates] = savedGridCell.GridCell;
            }
            
            foreach (GridCell cell in _grid.Values)
            {
                RecalculateCellNeighbours(cell);
            }
        }
        
        public void OnBeforeSerialize()
        {
            // Empty
        }

        public void OnAfterDeserialize()
        {
            BuildGrid();
        }

        [Serializable]
        private class SavedGridCell
        {
            [SerializeField]
            private GridCell _gridCell = null;

            [SerializeField]
            private Vector2 _coordinates = default;

            public GridCell GridCell => _gridCell;

            public Vector2 Coordinates => _coordinates;
            
            public SavedGridCell(GridCell gridCell, Vector2 coordinates)
            {
                _gridCell = gridCell;
                _coordinates = coordinates;
            }
        }
    }
}