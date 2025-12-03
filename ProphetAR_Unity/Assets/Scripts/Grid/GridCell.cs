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
        
        public GridCell LeftCell { get; private set; }
        
        public GridCell RightCell { get; private set; }
        
        public GridCell UpCell { get; private set; }
        
        public GridCell DownCell { get; private set; }
    }
}