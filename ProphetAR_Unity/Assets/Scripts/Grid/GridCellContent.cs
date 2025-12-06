using System;
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

        // Note that the grid cell reference will be assigned sometime in between Awake and Start.
        // We cannot access in Awake
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