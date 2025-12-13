using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public partial class CustomGrid : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Tooltip("(rows, columns)")]
        [SerializeField]
        [ReadOnly]
        private Vector2Int _gridDimensions = default;

        [SerializeField]
        [ReadOnly]
        private Vector2Int _minCoordinate = default;
        
        [SerializeField]
        [ReadOnly]
        private Vector2Int _maxCoordinate = default;
        
        [SerializeField]
        private GridSection _originGridSection = null;
        
        [SerializeField]
        [ReadOnly]
        private List<SavedGridCell> _savedGrid = null;
        
        private readonly Dictionary<Vector2Int, GridCell> _grid = new();
        
        public void OnAfterDeserialize()
        {
            BuildGrid();
        }
        
        private void BuildGrid()
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

        private void RecalculateCellNeighbours(GridCell cell)
        {
            if (_grid.TryGetValue(cell.Coordinates + Vector2Int.right, out GridCell right))
            {
                cell.SetRightCell(right);
            }
            
            if (_grid.TryGetValue(cell.Coordinates - Vector2Int.right, out GridCell left))
            {
                cell.SetLeftCell(left);
            }

            if (_grid.TryGetValue(cell.Coordinates + Vector2Int.up, out GridCell down))
            {
                cell.SetDownCell(down);
            }

            if (_grid.TryGetValue(cell.Coordinates - Vector2Int.up, out GridCell up))
            {
                cell.SetUpCell(up);
            }
        }
        
        public void OnBeforeSerialize()
        {
            // Empty
        }
    }
}