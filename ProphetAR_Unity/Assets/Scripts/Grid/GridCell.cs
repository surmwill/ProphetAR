using ProphetAR.Editor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProphetAR
{
    /// <summary>
    /// A transform representing the bottom left corner of the grid cell
    /// </summary>
    public partial class GridCell : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private GridSection _parentGridSection = null;

        [SerializeField]
        [ReadOnly]
        private Vector2 _coordinates;

        [SerializeField]
        private GridCellContent _cellContentPrefab;
        
        [SerializeField]
        [ReadOnly]
        private GridCellContent _cellContent;

        public GridSection ParentGridSection => _parentGridSection;

        public GridCellContent Content => _cellContent;

        public Vector2 Coordinates => _coordinates;

        public Vector2 Dimensions => _parentGridSection.CellDimensions;

        public Vector3 BotLeft => transform.position;

        public Vector3 BotRight => transform.parent.TransformPoint(transform.localPosition + Vector3.right * Dimensions.x);

        public Vector3 Middle => transform.parent.TransformPoint(transform.localPosition + Vector3.right * (Dimensions.x / 2f) + Vector3.forward * (Dimensions.y / 2f));

        public Vector3 TopLeft => transform.parent.TransformPoint(transform.localPosition + Vector3.forward * Dimensions.y);

        public Vector3 TopRight => transform.parent.TransformPoint(transform.localPosition + Vector3.right * Dimensions.x + Vector3.forward * Dimensions.y);
        
        public GridCell LeftCell { get; private set; }
        
        public GridCell RightCell { get; private set; }
        
        public GridCell UpCell { get; private set; }
        
        public GridCell DownCell { get; private set; }
        
        public void SetContent(GridCellContent contentPrefab)
        {
            if (_cellContentPrefab == contentPrefab)
            {
                return;
            }

            if (_cellContent != null)
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    EditorUtils.DestroyInEditMode(_cellContent.gameObject);
                }
                else
                {
                    Destroy(_cellContent.gameObject);   
                }
                #else 
                Destroy(_cellContent.gameObject);  
                #endif
            }

            if (contentPrefab != null)
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    _cellContent = (GridCellContent) PrefabUtility.InstantiatePrefab(contentPrefab, transform);
                }
                else
                {
                    _cellContent = Instantiate(contentPrefab, transform);     
                }
                #else
                _cellContent = Instantiate(contentPrefab, transform); 
                #endif
                
                _cellContent.SetGridCell(this);
            }
            
            _cellContentPrefab = contentPrefab;
        }
    }
}