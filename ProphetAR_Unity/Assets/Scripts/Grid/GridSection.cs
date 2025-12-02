using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public partial class GridSection : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Vector2 _gridDimensions = default;

        [SerializeField]
        private Vector2 _cellDimensions = default;
        
        [SerializeField]
        [ReadOnly]
        private Transform _cellsParent = null;

        [SerializeField]
        private GridCellContent _cellContent = null;

        public Vector2 GridDimensions => _gridDimensions;

        public Vector2 CellDimensions => _cellDimensions;
    }
}