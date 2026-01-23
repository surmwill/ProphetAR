using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    [ExecuteAlways]
    public class DebugGridCellBorder : MonoBehaviour
    {
        [SerializeField]
        private GridCellContent _gridCellContent = null;
        
        [SerializeField]
        private Image _borderImage = null;

        private const float ScaleDownForMargin = 0.95f;

        private bool _areEditModeListenersBound;

        private void Start()
        {
            // Run when the cell content has just been instantiated
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode && TryBindEditModeListeners())
            {
                EditorOnCellDimensionsChanged(_gridCellContent.Cell.Dimensions);
                EditorOnCellCoordinatesChanged(_gridCellContent.Cell.Coordinates);   
            }
            #endif
        }

        private void OnEnable()
        {
            // Run when the cell content already exists
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode && TryBindEditModeListeners())
            {
                EditorOnCellDimensionsChanged(_gridCellContent.Cell.Dimensions);
                EditorOnCellCoordinatesChanged(_gridCellContent.Cell.Coordinates);      
            }
            #endif
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                UnbindEditorListeners();   
            }
            #endif
        }

        #if UNITY_EDITOR
        private bool TryBindEditModeListeners()
        {
            if (!_areEditModeListenersBound && _gridCellContent != null && _gridCellContent.Cell != null)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged += EditorOnCellDimensionsChanged;
                _gridCellContent.Cell.EditorOnCellCoordinatesChanged += EditorOnCellCoordinatesChanged;
                _areEditModeListenersBound = true;

                return true;
            }

            return false;
        }
        #endif

        #if UNITY_EDITOR
        private void UnbindEditorListeners()
        {
            if (_areEditModeListenersBound)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged -= EditorOnCellDimensionsChanged;
                _gridCellContent.Cell.EditorOnCellCoordinatesChanged -= EditorOnCellCoordinatesChanged;
                _areEditModeListenersBound = false;
            }
        }
        #endif

        private void EditorOnCellDimensionsChanged(Vector2 dimensions)
        {
            transform.localScale = new Vector3(ScaleDownForMargin * dimensions.x, 1f, ScaleDownForMargin * dimensions.y);
        }

        private void EditorOnCellCoordinatesChanged(Vector2 newCoordinates)
        {
            _borderImage.color = newCoordinates == Vector2.zero ? Color.green : Color.white;
        }
    }
}
