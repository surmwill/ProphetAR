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
            _gridCellContent.Cell.OnCellDimensionsChanged += OnCellDimensionsChanged;
            OnCellDimensionsChanged(_gridCellContent.Cell.Dimensions);
        }

        private void OnDestroy()
        {
            _gridCellContent.Cell.OnCellDimensionsChanged -= OnCellDimensionsChanged;
        }

        private void OnCellDimensionsChanged(Vector2 dimensions)
        {
            transform.localScale = new Vector3(ScaleDownForMargin * dimensions.x, ScaleDownForMargin * dimensions.y, 1f);
        }
    }
}
