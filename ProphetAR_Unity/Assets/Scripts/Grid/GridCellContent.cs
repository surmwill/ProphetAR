using UnityEngine;

namespace ProphetAR
{
    [ExecuteAlways]
    public partial class GridCellContent : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private GridCell _cell = null;
        
        public GridCell Cell => _cell;

        // Note that the grid cell reference will be assigned sometime in between Awake and Start. We cannot access in Awake
        private void Start()
        {
            #if UNITY_EDITOR
            EditorStart();
            #endif
        }

        public void SetGridCell(GridCell cell)
        {
            #if UNITY_EDITOR
            EditorSetGridCell(_cell, cell);
            #endif
            
            _cell = cell;
            transform.position = cell.Middle;
        }

        private void OnDestroy()
        {
            #if UNITY_EDITOR
            EditorOnDestroy();
            #endif
        }
    }
}