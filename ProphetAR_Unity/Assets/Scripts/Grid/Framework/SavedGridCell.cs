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
        private Vector2Int _coordinates = default;

        public GridCell GridCell => _gridCell;

        public Vector2Int Coordinates => _coordinates;
            
        public SavedGridCell(GridCell gridCell, Vector2Int coordinates)
        {
            _gridCell = gridCell;
            _coordinates = coordinates;
        }
    }
}