using UnityEngine;

namespace ProphetAR
{
    public partial class GridSection : MonoBehaviour
    {
        [SerializeField]
        private Grid _parentGrid = null;
        
        [SerializeField]
        [ReadOnly]
        private Vector2 _sectionDimensions = default;

        [SerializeField]
        private Vector2 _cellDimensions = default;
        
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