using UnityEngine;

namespace ProphetAR
{
    [ExecuteAlways]
    public class GridCellTestBorder : MonoBehaviour
    {
        [SerializeField]
        private GridCellContent _gridCellContent = null;

        [SerializeField]
        private SpriteRenderer _spriteBorderRenderer = null;

        private const float ScaleDownForMargin = 0.95f;

        private bool _areEditModeListenersBound;

        private void Start()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                // Guaranteed that the grid cell has been hooked up by Start()
                if (!_areEditModeListenersBound)
                {
                    BindEditModeListeners();
                    EditorOnCellDimensionsChanged(_gridCellContent.Cell.Dimensions);
                    EditorOnCellCoordinatesChanged(_gridCellContent.Cell.Coordinates);   
                }
            }
            #endif
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                if (!_areEditModeListenersBound)
                {
                    BindEditModeListeners();
                    
                    // Not guaranteed that the grid cell has been hooked up by OnEnable()
                    if (_areEditModeListenersBound)
                    {
                        EditorOnCellDimensionsChanged(_gridCellContent.Cell.Dimensions);
                        EditorOnCellCoordinatesChanged(_gridCellContent.Cell.Coordinates);      
                    }
                }
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

        private void BindEditModeListeners()
        {
            if (!_areEditModeListenersBound && _gridCellContent.Cell != null)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged += EditorOnCellDimensionsChanged;
                _gridCellContent.Cell.EditorOnCellCoordinatesChanged += EditorOnCellCoordinatesChanged;
                _areEditModeListenersBound = true;
            }
        }

        private void UnbindEditorListeners()
        {
            if (_areEditModeListenersBound)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged -= EditorOnCellDimensionsChanged;
                _gridCellContent.Cell.EditorOnCellCoordinatesChanged -= EditorOnCellCoordinatesChanged;
                _areEditModeListenersBound = false;
            }
        }

        private void EditorOnCellDimensionsChanged(Vector2 dimensions)
        {
            transform.localScale = new Vector3(ScaleDownForMargin * dimensions.x, ScaleDownForMargin * dimensions.y, 1f);
        }

        private void EditorOnCellCoordinatesChanged(Vector2 newCoordinates)
        {
            _spriteBorderRenderer.color = newCoordinates == Vector2.zero ? Color.green : Color.white;
        }
    }
}
