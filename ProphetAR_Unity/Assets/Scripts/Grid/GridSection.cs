using UnityEngine;

namespace ProphetAR
{
    public partial class GridSection : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Grid _parentGrid = null;
        
        [SerializeField]
        [ReadOnly]
        private Vector2 _sectionDimensions = new(1, 1);

        [SerializeField]
        [ReadOnly]
        private Vector2 _cellDimensions = new(1, 1);
        
        [SerializeField]
        [ReadOnly]
        private Transform _cellsParent = null;

        [SerializeField]
        private GridCell _cellPrefab = null;
        
        [SerializeField]
        private GridCellContent _cellContentPrefab = null;

        public Grid ParentGrid => _parentGrid;

        public Vector2 SectionDimensions => _sectionDimensions;

        public Vector2 CellDimensions => _cellDimensions;
    }
}