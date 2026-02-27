using UnityEngine;

namespace ProphetAR
{
    public class GridSliceNavigation
    {
        public GridSlice Slice { get; }
        
        public GridSliceNavigation(GridSlice gridSlice)
        {
            Slice = gridSlice;
        }
        
        public (int row, int col) GridToDestinationSetCoords(Vector2Int gridCoords)
        {
            (int row, int col) destinationSetCoords = (gridCoords - Slice.Origin).ToTuple();
            return destinationSetCoords;
        }

        public Vector2Int DestinationSetToGridCoords((int row, int col) destinationSetCoords)
        {
            Vector2Int sliceCoords = Slice.Origin + destinationSetCoords.ToVector2Int();
            return sliceCoords;
        }
    }
}