using System;
using System.Linq;
using GridPathFinding;

namespace ProphetAR
{
    public class GridPainter
    {
        public CustomGrid Grid { get; }
        
        public GridPainter(CustomGrid grid)
        {
            Grid = grid;
        }
        
        public DisposableGridPainter ShowMovementArea(NavigationDestinationSet destinations, GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                if (destinations.Destinations.TryGetValue(gridCell.Coordinates.ToTuple(), out NavigationDestination destination))
                {
                    gridCell.GridCellPainter.ShowIsNavigable(true, destination.StepsRequired);
                }
                else
                {
                    gridCell.GridCellPainter.ShowIsNavigable(false);
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