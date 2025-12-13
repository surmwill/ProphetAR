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
        
        // Used for building the grid (positioning the actual GameObjects)
        [SerializeField]
        private GridSection _originGridSection = null;
        
        [SerializeField]
        [ReadOnly]
        private List<SavedGridCell> _savedGrid = null;
        
        public Dictionary<Vector2Int, GridCell> Cells { get; } = new();
        
        public GridCell this[Vector2Int coordinates] => Cells.GetValueOrDefault(coordinates);
        
        public void OnAfterDeserialize()
        {
            BuildGrid();
        }

        public GridSlice GetSlice(Vector2Int botLeft, Vector2Int dimensions)
        {
            return new GridSlice(this, botLeft, dimensions);
        }

        private void BuildGrid()
        {
            Cells.Clear();

            foreach (SavedGridCell savedGridCell in _savedGrid)
            {
                Cells[savedGridCell.Coordinates] = savedGridCell.GridCell;
            }
            
            foreach (GridCell cell in Cells.Values)
            {
                RecalculateCellNeighbours(cell);
            }
        }

        private void RecalculateCellNeighbours(GridCell cell)
        {
            if (Cells.TryGetValue(cell.Coordinates + Vector2Int.right, out GridCell right))
            {
                cell.SetRightCell(right);
            }
            
            if (Cells.TryGetValue(cell.Coordinates - Vector2Int.right, out GridCell left))
            {
                cell.SetLeftCell(left);
            }

            if (Cells.TryGetValue(cell.Coordinates + Vector2Int.up, out GridCell down))
            {
                cell.SetDownCell(down);
            }

            if (Cells.TryGetValue(cell.Coordinates - Vector2Int.up, out GridCell up))
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