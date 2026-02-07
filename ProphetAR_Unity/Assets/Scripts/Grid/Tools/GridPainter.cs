using System;
using GridOperations;

namespace ProphetAR
{
    public class GridPainter
    {
        public CustomGrid Grid { get; }
        
        public GridPainter(CustomGrid grid)
        {
            Grid = grid;
        }
        
        // Movement
        public DisposableGridPainter ShowMovementArea(NavigationDestinationSet destinations, GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                (int row, int col) normalizedCoordinates = (gridCell.Coordinates - gridSlice.TopLeft).ToTuple();
                if (destinations.Destinations.TryGetValue(normalizedCoordinates, out NavigationDestination destination))
                {
                    gridCell.GridCellPainter.ShowIsNavigable(true, destination.StepsRequired);
                }
            }

            return new DisposableGridPainter(() => ClearMovementArea(gridSlice));
        }
        
        public void ClearMovementArea(GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                gridCell.GridCellPainter.ShowIsNavigable(false);
            }
        }

        // Attack
        public DisposableGridPainter ShowAttackableArea(NavigationDestinationSet locations, GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                (int row, int col) normalizedCoordinates = (gridCell.Coordinates - gridSlice.TopLeft).ToTuple();
                if (locations.Destinations.TryGetValue(normalizedCoordinates, out NavigationDestination location))
                {
                    gridCell.GridCellPainter.ShowIsAttackable(true, location.StepsRequired);
                }
            }

            return new DisposableGridPainter(() => ClearAttackableArea(gridSlice));
        }
        
        public void ClearAttackableArea(GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                gridCell.GridCellPainter.ShowIsAttackable(false);
            }
        }

        /// <summary>
        /// Allows us to paint grid cells in using blocks { } instead of having paired ShowPaint() HidePaint() calls.
        /// This is optional and the ShowPaint() HidePaint() pattern can be used all the same.
        /// </summary>
        public class DisposableGridPainter : IDisposable
        {
            private readonly Action _cleanup;
            
            public DisposableGridPainter(Action cleanup)
            {
                _cleanup = cleanup;
            }

            public void Dispose()
            {
                _cleanup?.Invoke();
            }
        }
    }
}