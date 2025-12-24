using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using ProphetAR.Editor;
#endif

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

        [SerializeField]
        private Transform _charactersRoot = null;

        [Header("Debug")]
        [SerializeField]
        private bool _debugShowGridPointTypeIndicator = false;
        
        [SerializeField]
        private DebugCellGridPointTypeIndicator _obstacleIndicatorPrefab = null;

        [SerializeField]
        private DebugCellModificationStepIndicator _modificationStepIndicatorPrefab = null;
        
        [SerializeField]
        [ReadOnly]
        private DebugCellGridPointTypeIndicator _currentIndicator = null;

        [HideInInspector]
        [SerializeField]
        private GridPointType _lastGridPointType = GridPointType.Clear;
        
        public event Action<GridTransform> OnOccupierAdded;
        
        public event Action<GridTransform> OnOccupierRemoved;
        
        /// <summary>
        /// The GridCell this content falls under.
        /// Note that during content instantiation, this will assigned sometime in-between Awake/OnEnable and Start
        /// </summary>
        public GridCell Cell => _cell;

        /// <summary>
        /// At the lowest level our grid is just a 2D char array. This defines what char this cell is in that array (an obstacle, a free space, etc...)
        /// </summary>
        public GridPointProperties GridPointProperties => _gridPointProperties;
        
        /// <summary>
        /// Things currently occupying this cell, but could move elsewhere
        /// </summary>
        public List<GridTransform> OccupierTransforms { get; } = new();

        /// <summary>
        /// Characters occupying this cell
        /// </summary>
        public Transform CharactersRoot
        {
            get
            {
                if (_charactersRoot == null)
                {
                    Debug.LogWarning("Missing characters root");
                    return transform;
                }

                return _charactersRoot;
            }
        }
        
        /// <summary>
        /// The Characters currently occupying the cell
        /// </summary>
        public IEnumerable<Character> Characters => OccupierTransforms
            .Select(occupierTransform => occupierTransform.GridObject as Character)
            .Where(character => character != null);

        private bool _areEditModeListenersBound;
        private bool _lastGridPointTypeDirty;

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
        
        #region Occupiers
        
        public void AddOccupier(GridTransform addOccupier)
        {
            if (OccupierTransforms.Contains(addOccupier))
            {
                return;
            }
            
            OccupierTransforms.Add(addOccupier);
            OnOccupierAdded?.Invoke(addOccupier);
        }

        public void RemoveOccupier(GridTransform removeOccupier)
        {
            if (!OccupierTransforms.Contains(removeOccupier))
            {
                return;
            }

            OccupierTransforms.Remove(removeOccupier);
            OnOccupierRemoved?.Invoke(removeOccupier);
        }
        
        #endregion
        
        #region Debug
        
        public void DebugShowGridPointTypeIndicator(bool shouldShow)
        {
            _debugShowGridPointTypeIndicator = shouldShow;
            DebugUpdateShowGridPointTypeIndicator();
        }
        
        private void DebugUpdateShowGridPointTypeIndicator()
        {
            if (!_debugShowGridPointTypeIndicator)
            {
                if (_currentIndicator != null)
                {
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        EditorUtils.DestroyInEditMode(_currentIndicator.gameObject);   
                    }
                    else
                    {
                        Destroy(_currentIndicator.gameObject);
                    }
                    #else
                    Destroy(_currentIndicator.gameObject);
                    #endif
                    
                    _currentIndicator = null;
                }

                _lastGridPointTypeDirty = true;
                return;
            }
            
            if (_lastGridPointTypeDirty || _lastGridPointType != GridPointProperties.GridPointType)
            {
                if (_currentIndicator != null)
                {
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        EditorUtils.DestroyInEditMode(_currentIndicator.gameObject);   
                    }
                    else
                    {
                        Destroy(_currentIndicator.gameObject);
                    }
                    #else
                    Destroy(_currentIndicator.gameObject);
                    #endif
                    
                    _currentIndicator = null;
                }

                switch (GridPointProperties.GridPointType)
                {
                    case GridPointType.Obstacle:
                        _currentIndicator = Instantiate(_obstacleIndicatorPrefab, transform);
                        break;
                    
                    case GridPointType.ModificationStep:
                        _currentIndicator = Instantiate(_modificationStepIndicatorPrefab, transform);
                        break;
                }

                _lastGridPointType = GridPointProperties.GridPointType;
                _lastGridPointTypeDirty = false;
            }

            if (_currentIndicator != null)
            {
                _currentIndicator.SetGridPoint(GridPointProperties.GridPoint);
            }
        }
        
        #endregion
    }
}