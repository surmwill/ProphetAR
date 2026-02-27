using GridOperations;

namespace ProphetAR
{
    public class GridSliceNavigationDestination : GridSliceNavigation
    {
        public NavigationInstructionSet Destination { get; }

        public GridSliceNavigationDestination(GridSlice gridSlice, NavigationInstructionSet destination) : base(gridSlice)
        {
            Destination = destination;
        }
    }
}