using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// A transform representing the bottom left corner of the grid cell
    /// </summary>
    public partial class GridCell : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private GridSection _parentGridSection = null;

        [SerializeField]
        [ReadOnly]
        private Vector2 _coordinates;

        [SerializeField]
        private GridCellContent _cellContentPrefab;
        
        [SerializeField]
        [ReadOnly]
        private GridCellContent _cellContent;

        public GridSection ParentGridSection => _parentGridSection;

        public GridCellContent Content => _cellContent;

        public Vector2 Coordinates => _coordinates;

        public Vector2 Dimensions => _parentGridSection.CellDimensions;

        public Vector3 BotLeft => transform.position;

        public Vector3 BotRight => transform.TransformPoint(transform.localPosition + Vector3.right * Dimensions.x);

        public Vector3 Middle => transform.TransformPoint(transform.localPosition + Vector3.right * (Dimensions.x / 2f) + Vector3.forward * (Dimensions.y / 2f));

        public Vector3 TopLeft => transform.TransformPoint(transform.localPosition + Vector3.forward * Dimensions.y);

        public Vector3 TopRight => transform.TransformPoint(transform.localPosition + Vector3.right * Dimensions.x + Vector3.forward * Dimensions.y);
        
        public GridCell LeftCell { get; private set; }
        
        public GridCell RightCell { get; private set; }
        
        public GridCell UpCell { get; private set; }
        
        public GridCell DownCell { get; private set; }
    }
}