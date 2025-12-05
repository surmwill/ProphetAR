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

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                BindDimensionsChangedListenerIfNeeded();
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
                BindDimensionsChangedListenerIfNeeded();
            }
            #endif
        }

        public void SetGridCell(GridCell cell)
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                UnbindDimensionsChangedListener();
            }
            #endif
            
            _cell = cell;
            CenterTransform();
            
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                BindDimensionsChangedListenerIfNeeded();
            }
            #endif
        }

        private void CenterTransform()
        {
            transform.position = _cell.Middle;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                UnbindDimensionsChangedListener();   
            }
            #endif
        }
    }
}