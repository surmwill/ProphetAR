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
            foreach (NavigationDestination navigationDestination in possibleDestinations)
            {
                GridCell gridCell = Grid[gridSlice.Origin + navigationDestination.Position.ToVector2Int()];
                
            }
        }
    }
}