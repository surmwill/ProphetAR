#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public partial class Grid : ISerializationCallbackReceiver
    {
        private const string CellsParentName = "Cells";
        private HashSet<GridCell> _serializedCellsLookup = null;

        public void SetCellDimensions(Vector2 cellDimensions)
        {
            _cellDimensions = cellDimensions;
            foreach (GridCell cell in _serializedCells)
            {
                cell.UpdateDimensions(cellDimensions);
            }
        }

        public void AddCell(GridCell cell)
        {
            if (cell.transform.parent != _cellsParent)
            {
                Debug.LogError($"All cells must be a child of `{_cellsParent.name}`");
                return;
            }
            
            if (_serializedCellsLookup.Add(cell))
            {
                _serializedCells.Add(cell);
            }
        }

        public void AddCells(IEnumerable<GridCell> cells)
        {
            foreach (GridCell cell in cells)
            {
                AddCell(cell);
            }
        }

        public void RemoveCell(GridCell gridCell)
        {
            if (_serializedCellsLookup.Remove(gridCell))
            {
                _serializedCells.Remove(gridCell);
            }
        }

        public void RemoveCells(IEnumerable<GridCell> toRemoveGridCells)
        {
            HashSet<GridCell> removed = new HashSet<GridCell>();
            foreach (GridCell gridCell in toRemoveGridCells)
            {
                if (_serializedCellsLookup.Remove(gridCell))
                {
                    removed.Add(gridCell);
                }
            }
            _serializedCells.RemoveAll(cell => removed.Contains(cell));
        }

        public void AddOrigin()
        {
            if (_originCell != null)
            {
                Debug.LogWarning("Origin already present");
                return;
            }
            
            AddCellsParentIfNeeded();
            GridCell.Create(Vector3.zero, _cellDimensions);
        }

        private void AddCellsParentIfNeeded()
        {
            if (_cellsParent != null)
            {
                return;
            }

            _cellsParent = new GameObject(CellsParentName).transform;
            _cellsParent.SetParent(gameObject.transform);
            _cellsParent.SetLocalPositionAndRotation(Vector3.one, Quaternion.identity);
            _cellsParent.localScale = Vector3.one;
        }
        
        private void OnValidate()
        {
            AddCellsParentIfNeeded();
        }

        public void OnBeforeSerialize()
        {
            // Empty
        }

        public void OnAfterDeserialize()
        {
            _serializedCellsLookup = new HashSet<GridCell>(_serializedCells);
        }
    }
}

#endif