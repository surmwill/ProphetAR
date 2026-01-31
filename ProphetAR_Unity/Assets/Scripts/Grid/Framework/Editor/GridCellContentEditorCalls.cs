using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace ProphetAR
{
    public partial class GridCellContent
    {
        private readonly List<IGridCellContentDimensionsChangedListener> _dimensionsChangedListeners = new();
        private readonly List<IGridCellContentCoordinatesChangedListener> _coordinatesChangedListeners = new();
        
        public void OnCellDimensionsChanged(Vector2 newDimensions)
        {
            if (!ApplicationUtils.IsEditMode)
            {
                return;
            }
            
            transform.position = _cell.Middle;
            _debugGridPointIndicatorParent.localScale = new Vector3(newDimensions.x, 1f, newDimensions.y);

            foreach (IGridCellContentDimensionsChangedListener listener in _dimensionsChangedListeners)
            {
                listener.OnDimensionsChanged(newDimensions);
            }
        }

        public void OnCellCoordinatesChanged(Vector2Int newCoordinates)
        {
            if (!ApplicationUtils.IsEditMode)
            {
                return;
            }
            
            foreach (IGridCellContentCoordinatesChangedListener listener in _coordinatesChangedListeners)
            {
                listener.OnCoordinatesChanged(newCoordinates);
            }
        }

        public void RegisterOnDimensionsChangedListener(IGridCellContentDimensionsChangedListener listener)
        {
            _dimensionsChangedListeners.Add(listener);
            if (_cell != null)
            {
                listener.OnDimensionsChanged(_cell.Dimensions);
            }
        }

        public void UnregisterOnDimensionsChangedListener(IGridCellContentDimensionsChangedListener listener)
        {
            _dimensionsChangedListeners.Remove(listener);
        }

        public void RegisterOnCoordinatesChangedListener(IGridCellContentCoordinatesChangedListener listener)
        {
            _coordinatesChangedListeners.Add(listener);
            if (_cell != null)
            {
                listener.OnCoordinatesChanged(_cell.Coordinates);
            }
        }

        public void UnregisterOnCoordinatesChangedListener(IGridCellContentCoordinatesChangedListener listener)
        {
            _coordinatesChangedListeners.Remove(listener);
        }
        
        private void OnValidate()
        {
            _gridPointProperties.OnValidate();
            DebugUpdateShowGridPointTypeIndicator();
        }
    }
}
#endif