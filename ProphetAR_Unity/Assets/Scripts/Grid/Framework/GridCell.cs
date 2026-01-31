using System;
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
        private BoxCollider _cellCollider;

        public GridSection GridSection => _gridSection;
        
        public GridCellContent Content => _cellContent;

        public GridPointProperties GridPointProperties => Content.GridPointProperties;
        
        public GridCellPainter GridCellPainter => Content.GridCellPainter;

        public Vector2Int Coordinates => _coordinates;

        public Vector2 Dimensions => _gridSection != null ? _gridSection.CellDimensions : Vector2.one;

        // World corners
        public Vector3 BotLeft => transform.position;

        public Vector3 BotRight => transform.position + Vector3.right * Dimensions.x;

        public Vector3 Middle => transform.position + Vector3.right * (Dimensions.x / 2f) + Vector3.forward * (Dimensions.y / 2f);

        public Vector3 TopLeft => transform.position + Vector3.forward * Dimensions.y;

        public Vector3 TopRight => transform.position + Vector3.right * Dimensions.x + Vector3.forward * Dimensions.y;
        
        // Adjacent cells
        public GridCell LeftCell { get; private set; }
        
        public GridCell RightCell { get; private set; }
        
        public GridCell UpCell { get; private set; }
        
        public GridCell DownCell { get; private set; }
        
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
        
        public void SetContent(GridCellContent contentPrefab)
        {
            if (_cellContentParent == null)
            {
                throw new InvalidOperationException("Missing parent transform for the grid cell content");
            }
            
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
            
            // _lastCellContent is only used in OnValidate to detect changes in the serialized prefab we drag in.
            // Since that isn't happening here (we're doing it through code), we set them to the same thing to detect that in the future.
            _cellContentPrefab = contentPrefab;
            _lastCellContentPrefab = contentPrefab;
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}