using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProphetAR
{
    /// <summary>
    /// Base container for everything in a grid cell. Defines the bottom left corner of it.
    /// Other than very basic information like coordinates, most things will be found under GridCellContent.
    /// Its primary purpose is to be a stable parent for GridCellContent that can be dynamically swapped.
    /// </summary>
    public partial class GridCell : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private GridSection _gridSection = null;

        [SerializeField]
        [ReadOnly]
        private Vector2Int _coordinates;

        [SerializeField]
        private Transform _cellContentParent = null;
        
        [SerializeField]
        private GridCellContent _cellContentPrefab;

        [SerializeField]
        private GridCellContent _lastCellContentPrefab;
        
        [ReadOnly]
        [SerializeField]
        private GridCellContent _cellContent;

        [SerializeField]
        private GridCellPainter _cellPainter;

        public GridSection GridSection => _gridSection;
        
        public GridCellContent Content => _cellContent;

        public GridPointProperties GridPointProperties => Content.GridPointProperties;

        /// <summary>
        /// Paints overlays (ex: green if the cell is navigable, red for being in attack range)
        /// </summary>
        public GridCellPainter GridCellPainter => _cellPainter;

        public Vector2Int Coordinates => _coordinates;

        public Vector2 Dimensions => _gridSection.CellDimensions;

        // World corners
        public Vector3 BotLeft => transform.position;

        public Vector3 BotRight => transform.parent.TransformPoint(transform.localPosition + Vector3.right * Dimensions.x);

        public Vector3 Middle => transform.parent.TransformPoint(transform.localPosition + Vector3.right * (Dimensions.x / 2f) + Vector3.forward * (Dimensions.y / 2f));

        public Vector3 TopLeft => transform.parent.TransformPoint(transform.localPosition + Vector3.forward * Dimensions.y);

        public Vector3 TopRight => transform.parent.TransformPoint(transform.localPosition + Vector3.right * Dimensions.x + Vector3.forward * Dimensions.y);
        
        // Adjacent cells
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
            
            DestroyUtils.DestroyAnywhereChildren(_cellContentParent.gameObject);

            if (contentPrefab != null)
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    _cellContent = (GridCellContent) PrefabUtility.InstantiatePrefab(contentPrefab, _cellContentParent);
                }
                else
                {
                    _cellContent = Instantiate(contentPrefab, _cellContentParent);     
                }
                #else
                _cellContent = Instantiate(contentPrefab, _cellContentParent); 
                #endif
                
                _cellContent.SetGridCell(this);
            }
            
            _cellContentPrefab = contentPrefab;
            _lastCellContentPrefab = contentPrefab;
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}