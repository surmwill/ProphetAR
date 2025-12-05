#if UNITY_EDITOR
using UnityEngine;

namespace ProphetAR
{
    public partial class GridCellContent
    {
        private bool _isListeningToDimensionsChanged;

        private void BindDimensionsChangedListenerIfNeeded()
        {
            if (!_isListeningToDimensionsChanged && _cell != null)
            {
                _cell.EditorOnCellDimensionsChanged += EditorOnCellDimensionsChanged;
                _isListeningToDimensionsChanged = true;
            }
        }

        private void UnbindDimensionsChangedListener()
        {
            if (_isListeningToDimensionsChanged)
            {
                _cell.EditorOnCellDimensionsChanged -= EditorOnCellDimensionsChanged;
                _isListeningToDimensionsChanged = false;
            }
        }
        
        private void EditorOnCellDimensionsChanged(Vector2 newDimensions)
        {
            CenterTransform();
        }
    }
}
#endif