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
        
        public void ShowMovementArea(NavigationDestinationSet possibleDestinations, GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                if (possibleDestinations.Destinations.TryGetValue(gridCell.Coordinates.ToTuple(), out NavigationDestination destination))
                {
                    gridCell.GridCellPainter.ShowIsNavigable(true, destination.StepsRequired);
                }
                else
                {
                    gridCell.GridCellPainter.ShowIsNavigable(false);
                }
            }
        }

        public void ClearMovementArea(GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                gridCell.GridCellPainter.ShowIsNavigable(false);
            }
        }
    }
}