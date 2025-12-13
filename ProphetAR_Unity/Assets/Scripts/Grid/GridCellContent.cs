using UnityEngine;

namespace ProphetAR
{
    [ExecuteAlways]
    public partial class GridCellContent : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private GridCell _cell = null;

        [SerializeField]
        private GridPointProperties _gridPointProperties = new();
        
        /// <summary>
        /// Note that this will assigned sometime in-between Awake/OnEnable and Start.
        /// Subsequently, a reference to the Grid is first available in Start
        /// </summary>
        public GridCell Cell => _cell;

        /// <summary>
        /// Logically these would make more sense in GridCell, but I think realistically you would want to set this along with the cell's content.
        /// (Ex: setting this to an obstacle while you're adding in the 3D model of a tree)
        /// </summary>
        public GridPointProperties GridPointProperties => _gridPointProperties;

        private bool _areEditModeListenersBound;

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                if (!_areEditModeListenersBound)
                {
                    BindEditModeListeners();   
                }
            }
            #endif
        }
        
        private void Start()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                if (!_areEditModeListenersBound)
                {
                    BindEditModeListeners();   
                }
            }
            #endif
        }

        public void SetGridCell(GridCell cell)
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                UnbindEditModeListeners();
            }
            #endif
            
            _cell = cell;
            OnCellDimensionsChanged(_cell.Dimensions);
            
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                BindEditModeListeners();
            }
            #endif
        }
        
        private void OnCellDimensionsChanged(Vector2 newDimensions)
        {
            transform.position = _cell.Middle;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                UnbindEditModeListeners();   
            }
            #endif
        }
    }
}