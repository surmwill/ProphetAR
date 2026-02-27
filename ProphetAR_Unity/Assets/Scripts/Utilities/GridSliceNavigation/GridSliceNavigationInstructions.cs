using GridOperations;

namespace ProphetAR
{
    public class GridSliceNavigationInstructions : GridSliceNavigation
    {
        public NavigationInstructionSet Instructions { get; }

        public GridSliceNavigationInstructions(GridSlice gridSlice, NavigationInstructionSet instructions) : base(gridSlice)
        {
            Instructions = instructions;
        }
    }
}