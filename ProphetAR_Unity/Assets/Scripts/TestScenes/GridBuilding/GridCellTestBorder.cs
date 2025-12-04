using UnityEngine;

namespace ProphetAR
{
    [ExecuteAlways]
    public class GridCellTestBorder : MonoBehaviour
    {
        [SerializeField]
        private GridCellContent _gridCellContent = null;

        private const float ScaleDownForMargin = 0.95f;

        private void Start()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged += EditorContentOnCellDimensionsChanged;  
                EditorContentOnCellDimensionsChanged(_gridCellContent.Cell.Dimensions);
            }
            #endif
        }

        private void OnDestroy()
        {
            #if UNITY_EDITOR
            if (ApplicationUtils.IsEditMode)
            {
                _gridCellContent.Cell.EditorOnCellDimensionsChanged -= EditorContentOnCellDimensionsChanged;   
            }
            #endif
        }

        private void EditorContentOnCellDimensionsChanged(Vector2 dimensions)
        {
            transform.localScale = new Vector3(ScaleDownForMargin * dimensions.x, ScaleDownForMargin * dimensions.y, 1f);
        }
    }
}
