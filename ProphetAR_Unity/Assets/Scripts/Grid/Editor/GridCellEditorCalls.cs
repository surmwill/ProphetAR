#if UNITY_EDITOR
using ProphetAR.Editor;
using UnityEngine;

namespace ProphetAR
{
    // Editor only calls. Extra functionality is needed for building the grid through the editor
    public partial class GridCell
    {
        private GridCellContent _lastCellContentPrefab;
        
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

        public void SetCoordinates(Vector2 coordinates)
        {
            _coordinates = coordinates;
        }

        public void SetCellContent(GridCellContent contentPrefab)
        {
            if (_cellContentPrefab == contentPrefab)
            {
                return;
            }

            if (_cellContent != null)
            {
                #if UNITY_EDITOR
                EditorUtils.OnValidateDestroy(_cellContent.gameObject);
                #else
                Destroy(_cellContent.gameObject);
                #endif
            }

            _cellContent = Instantiate(contentPrefab, transform);
            _cellContentPrefab = contentPrefab;
        }

        private void OnValidate()
        {
            if (_lastCellContentPrefab != null && _lastCellContentPrefab != _cellContentPrefab)
            {
                if (_cellContent != null)
                {
                    EditorUtils.OnValidateDestroy(_cellContent.gameObject);   
                }

                if (_cellContentPrefab != null)
                {
                    _cellContent = Instantiate(_cellContentPrefab, transform);
                }
            }

            _lastCellContentPrefab = _cellContentPrefab;
        }
    }
}
#endif