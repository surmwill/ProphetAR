using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Represents a rectangular piece of the grid (used for building).
    /// These grid sections are glued together to construct the entire level.
    /// This way for example, we can change one room of the map without requiring new assets for the entire level.
    /// </summary>
    public partial class GridSection : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private CustomGrid _parentGrid = null;
        
        [SerializeField]
        [ReadOnly]
        private Vector2 _sectionDimensions = new(2, 2);

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

        [SerializeField]
        private List<GridSnap> _gridSnaps = null;

        public List<GridSnap> GridSnaps => _gridSnaps;

        public CustomGrid ParentGrid => _parentGrid;

        public Vector2 SectionDimensions => _sectionDimensions;

        public Vector2 CellDimensions => _cellDimensions;
    }
}