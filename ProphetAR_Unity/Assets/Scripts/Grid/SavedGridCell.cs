using System;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public class SavedGridCell
    {
        [SerializeField]
        private GridCell _gridCell = null;

        [SerializeField]
        private Vector2 _coordinates = default;

        public GridCell GridCell => _gridCell;

        public Vector2 Coordinates => _coordinates;
            
        public SavedGridCell(GridCell gridCell, Vector2 coordinates)
        {
            _gridCell = gridCell;
            _coordinates = coordinates;
        }
    }
}