#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    // Editor only calls. Extra functionality is needed for building the grid through the editor
    public partial class GridCell
    {
        public void SetGridSection(GridSection gridSection)
        {
            _gridSection = gridSection;
        }

        public void SetCoordinates(Vector2Int coordinates)
        {
            _coordinates = coordinates;
            
            if (_cellContent != null)
            {
                _cellContent.OnCellCoordinatesChanged(coordinates);
            }
            
            EditorUtility.SetDirty(this);
        }

        public void SetDimensions(Vector2 newDimensions)
        {
            OnCellDimensionsChanged();
            
            if (_cellContent != null)
            {
                _cellContent.OnCellDimensionsChanged(newDimensions);
            }
            
            EditorUtility.SetDirty(this);
        }

        private void CheckCellContentChange()
        {
            if (_lastCellContentPrefab == _cellContentPrefab)
            {
                return;
            }
            
            if (_cellContentParent == null)
            {
                Debug.LogWarning("Missing parent transform for cell content");   
                return;
            }
            
            DestroyUtils.DestroyAnywhereChildren(_cellContentParent.gameObject);
            
            if (_cellContentPrefab != null)
            {
                _cellContent = (GridCellContent) PrefabUtility.InstantiatePrefab(_cellContentPrefab, _cellContentParent);
                _cellContent.SetGridCell(this);
            }

            _lastCellContentPrefab = _cellContentPrefab; 
        }

        private void OnCellDimensionsChanged()
        {
            Vector3 scale = new Vector3(Dimensions.x, 1f, Dimensions.y);
            
            if (_cellPainter != null)
            {
                _cellPainter.transform.position = Middle;
                _cellPainter.transform.localScale = scale;
            }
            
            if (_cellCollider != null)
            {
                _cellCollider.transform.position = Middle;
                _cellCollider.transform.localScale = scale;
            }
        }

        private void OnValidate()
        {
            CheckCellContentChange();
            OnCellDimensionsChanged();
        }
    }
}
#endif