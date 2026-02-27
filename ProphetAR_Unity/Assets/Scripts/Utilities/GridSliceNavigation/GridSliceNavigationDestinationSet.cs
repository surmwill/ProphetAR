using GridOperations;

namespace ProphetAR
{
    public class GridSliceNavigationDestinationSet : GridSliceNavigation
    {
        public NavigationDestinationSet DestinationSet { get; }
        
        public GridSliceNavigationDestinationSet(GridSlice slice, NavigationDestinationSet destinationSet) : base(slice)
        {
            DestinationSet = destinationSet;
        }
    }
}