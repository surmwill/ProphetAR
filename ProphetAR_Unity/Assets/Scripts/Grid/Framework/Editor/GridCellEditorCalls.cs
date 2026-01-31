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
            OnDimensionsChanged();
            
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

        private void OnDimensionsChanged()
        {
            MatchCellPositionAndDimensions(_cellCollider.transform);
        }
        
        private void MatchCellPositionAndDimensions(Transform t)
        {
            t.position = Middle;
            t.localScale = new Vector3(Dimensions.x, 1f, Dimensions.y);
        }

        private void OnValidate()
        {
            CheckCellContentChange();
        }
    }
}
#endif