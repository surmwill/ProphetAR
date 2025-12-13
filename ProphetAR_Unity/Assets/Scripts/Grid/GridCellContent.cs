using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Parent component for the content of a grid cells.
    /// Represents the middle of the cell and provides useful references to the grid cell for everything that references it
    /// </summary>
    [ExecuteAlways]
    public partial class GridCellContent : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private GridCell _cell = null;

        [SerializeField]
        private GridPointProperties _gridPointProperties = new();
        
        /// <summary>
        /// The GridCell this content falls under.
        /// Note that during content instantiation, this will assigned sometime in-between Awake/OnEnable and Start
        /// </summary>
        public GridCell Cell => _cell;

        /// <summary>
        /// At the lowest level our grid is just a 2D char array. This defines what char this cell is in that array (an obstacle, a free space, etc...)
        /// </summary>
        public GridPointProperties GridPointProperties => _gridPointProperties;
        
        public IEnumerable<Character> Characters => _occupierTransforms
            .Select(occupierTransform => occupierTransform.GridObject as Character)
            .Where(character => character != null);
        
        /// The things currently occupying this cell but might move later
        private readonly List<GridTransform> _occupierTransforms = new();

        public event Action<GridTransform> OnOccupierAdded;
        public event Action<GridTransform> OnOccupierRemoved; 

        private bool _areEditModeListenersBound;

        public IEnumerable<GridTransform> GetOccupiers()
        {
            return _occupierTransforms;
        }

        public void AddOccupier(GridTransform addOccupier)
        {
            if (_occupierTransforms.Contains(addOccupier))
            {
                return;
            }
            
            _occupierTransforms.Add(addOccupier);
            OnOccupierAdded?.Invoke(addOccupier);
        }

        public void RemoveOccupier(GridTransform removeOccupier)
        {
            if (!_occupierTransforms.Contains(removeOccupier))
            {
                return;
            }

            _occupierTransforms.Remove(removeOccupier);
            OnOccupierRemoved?.Invoke(removeOccupier);
        }

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