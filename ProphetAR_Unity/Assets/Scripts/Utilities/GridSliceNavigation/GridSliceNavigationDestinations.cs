using GridOperations;

namespace ProphetAR
{
    public class GridSliceNavigationDestinations : GridSliceNavigation
    {
        public NavigationDestinationSet Destinations { get; }
        
        public GridSliceNavigationDestinations(GridSlice slice, NavigationDestinationSet destinations) : base(slice)
        {
            Destinations = destinations;
        }
    }
}