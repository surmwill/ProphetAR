using System;
using UnityEngine;

namespace ProphetAR
{
    [ExecuteAlways]
    public class GridCellTestBorder : MonoBehaviour
    {
        [SerializeField]
        private GridCellContent _gridCellContent = null;

        private const float ScaleDownForMargin = 0.95f;

        private bool _isListenerBound;

        private void Start()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                BindCellDimensionsChangedListenerIfNeeded();
                EditorContentOnCellDimensionsChanged(_gridCellContent.Cell.Dimensions);
            }
            #endif
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                BindCellDimensionsChangedListenerIfNeeded();   
            }
            #endif
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                UnbindCellDimensionsChangedListener();   
            }
            #endif
        }

        private void BindCellDimensionsChangedListenerIfNeeded()
        {
            if (!_isListenerBound && _gridCellContent.Cell != null)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged += EditorContentOnCellDimensionsChanged;
                _isListenerBound = true;
            }
        }

        private void UnbindCellDimensionsChangedListener()
        {
            if (_isListenerBound)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged -= EditorContentOnCellDimensionsChanged;
                _isListenerBound = false;
            }
        }

        private void EditorContentOnCellDimensionsChanged(Vector2 dimensions)
        {
            transform.localScale = new Vector3(ScaleDownForMargin * dimensions.x, ScaleDownForMargin * dimensions.y, 1f);
            Debug.Log("NEW DIMENSIONS " + new Vector3(ScaleDownForMargin * dimensions.x, ScaleDownForMargin * dimensions.y, 1f));
        }
    }
}
