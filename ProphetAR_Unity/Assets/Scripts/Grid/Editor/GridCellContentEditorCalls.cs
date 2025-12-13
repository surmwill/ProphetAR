#if UNITY_EDITOR

namespace ProphetAR
{
    public partial class GridCellContent
    {
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

        private void OnValidate()
        {
            _gridPointProperties.OnValidate();
        }
    }
}
#endif