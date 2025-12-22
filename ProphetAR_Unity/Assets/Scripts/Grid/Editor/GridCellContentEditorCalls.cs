using ProphetAR.Editor;

#if UNITY_EDITOR

namespace ProphetAR
{
    public partial class GridCellContent
    {
        private bool _lastGridPointTypeDirty;
        
        private void BindEditModeListeners()
        {
            if (!_areEditModeListenersBound && _cell != null)
            {
                _cell.EditorOnCellDimensionsChanged += OnCellDimensionsChanged;
                _areEditModeListenersBound = true;
            }
        }

        private void UnbindEditModeListeners()
        {
            if (_areEditModeListenersBound)
            {
                _cell.EditorOnCellDimensionsChanged -= OnCellDimensionsChanged;
                _areEditModeListenersBound = false;
            }
        }

        private void DebugVisualizeGridPointType()
        {
            if (!_debugShowGridPointTypeIndicator)
            {
                if (_currentIndicator != null)
                {
                    EditorUtils.DestroyInEditMode(_currentIndicator.gameObject);
                    _currentIndicator = null;
                }

                _lastGridPointTypeDirty = true;
                return;
            }
            
            if (_lastGridPointTypeDirty || _lastGridPointType != GridPointProperties.GridPointType)
            {
                if (_currentIndicator != null)
                {
                    EditorUtils.DestroyInEditMode(_currentIndicator.gameObject);
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

        private void OnValidate()
        {
            _gridPointProperties.OnValidate();
            DebugVisualizeGridPointType();
        }
    }
}
#endif