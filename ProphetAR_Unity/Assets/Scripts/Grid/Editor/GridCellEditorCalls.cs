#if UNITY_EDITOR
using System;
using ProphetAR.Editor;
using UnityEngine;

namespace ProphetAR
{
    // Editor only calls. Extra functionality is needed for building the grid through the editor
    public partial class GridCell
    {
        private GridCellContent _lastCellContentPrefab;

        public event Action<Vector2> OnCellDimensionsChanged; 
        
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

        public void SetContent(GridCellContent contentPrefab)
        {
            if (_cellContentPrefab == contentPrefab)
            {
                return;
            }

            if (_cellContent != null)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    EditorUtils.DestroyInEditMode(_cellContent.gameObject);
                }
                else
                {
                    Destroy(_cellContent.gameObject);   
                }
            }

            if (contentPrefab != null)
            {
                _cellContent = Instantiate(contentPrefab, transform);  
                _cellContent.SetGridCell(this);
            }
            
            _cellContentPrefab = contentPrefab;
        }

        public void NotifyCellDimensionsChanged(Vector2 newDimensions)
        {
            OnCellDimensionsChanged?.Invoke(newDimensions);
        }

        private void OnValidate()
        {
            if (_lastCellContentPrefab != null && _lastCellContentPrefab != _cellContentPrefab)
            {
                if (_cellContent != null)
                {
                    EditorUtils.DestroyInEditMode(_cellContent.gameObject);   
                }

                if (_cellContentPrefab != null)
                {
                    _cellContent = Instantiate(_cellContentPrefab, transform);
                    _cellContent.SetGridCell(this);
                }
            }

            _lastCellContentPrefab = _cellContentPrefab;
        }
    }
}
#endif