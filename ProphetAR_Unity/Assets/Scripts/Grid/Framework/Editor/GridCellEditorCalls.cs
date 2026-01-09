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
        public event Action<Vector2> EditorOnCellDimensionsChanged;
        public event Action<Vector2> EditorOnCellCoordinatesChanged;
        
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

        public void SetGridSection(GridSection gridSection)
        {
            _gridSection = gridSection;
        }

        public void SetCoordinates(Vector2Int coordinates)
        {
            _coordinates = coordinates;
            EditorOnCellCoordinatesChanged?.Invoke(coordinates);
        }

        public void EditorNotifyCellDimensionsChanged(Vector2 newDimensions)
        {
            EditorOnCellDimensionsChanged?.Invoke(newDimensions);
        }

        private void OnValidate()
        {
            if (_lastCellContentPrefab != _cellContentPrefab)
            {
                if (_lastCellContentPrefab != null && _cellContentPrefab != null)
                {
                    Debug.Log(_lastCellContentPrefab.gameObject.name + " " + _cellContentPrefab.gameObject.name + " " + (_lastCellContentPrefab == _cellContentPrefab));   
                }       
                
                if (_cellContent != null)
                {
                    DestroyUtils.DestroyAnywhereChildren(_cellContentParent.gameObject);
                }

                if (_cellContentPrefab != null)
                {
                    _cellContent = (GridCellContent) PrefabUtility.InstantiatePrefab(_cellContentPrefab, _cellContentParent);
                    _cellContent.SetGridCell(this);
                }
            }

            _lastCellContentPrefab = _cellContentPrefab;
        }
    }
}
#endif