#if UNITY_EDITOR
using System;
using ProphetAR.Editor;
using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    // Editor only calls. Extra functionality is needed for building the grid through the editor
    public partial class GridCell
    {
        private GridCellContent _lastCellContentPrefab;
        private bool _checkedLastCellContentPrefab;

        public event Action<Vector2> EditorOnCellDimensionsChanged; 
        
        public void SetLeftCell(GridCell cell)
        {
            LeftCell = cell;
        }
        
        public void SetRightCell(GridCell cell)
        {
            LeftCell = cell;
        }
        
        public void SetUpCell(GridCell cell)
        {
            LeftCell = cell;
        }
        
        public void SetDownCell(GridCell cell)
        {
            LeftCell = cell;
        }

        public void SetParentGridSection(GridSection gridSection)
        {
            _parentGridSection = gridSection;
        }

        public void SetCoordinates(Vector2 coordinates)
        {
            _coordinates = coordinates;
        }

        public void EditorNotifyCellDimensionsChanged(Vector2 newDimensions)
        {
            EditorOnCellDimensionsChanged?.Invoke(newDimensions);
        }

        private void OnValidate()
        {
            if (!_checkedLastCellContentPrefab)
            {
                _checkedLastCellContentPrefab = true;
                _lastCellContentPrefab = _cellContentPrefab;
                return;
            }
            
            if (_lastCellContentPrefab != _cellContentPrefab)
            {
                if (_cellContent != null)
                {
                    EditorUtils.DestroyInEditMode(_cellContent.gameObject);   
                }

                if (_cellContentPrefab != null)
                {
                    _cellContent = (GridCellContent) PrefabUtility.InstantiatePrefab(_cellContentPrefab, transform);
                    _cellContent.SetGridCell(this);
                }
            }

            _lastCellContentPrefab = _cellContentPrefab;
        }
    }
}
#endif