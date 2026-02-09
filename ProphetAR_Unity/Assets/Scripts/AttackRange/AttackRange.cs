using System.Collections.Generic;
using GridOperations;

namespace ProphetAR
{
    public readonly struct AttackRange
    {
        // Non-null
        public Dictionary<(int row, int col), int> Locations { get; }

        public (int row, int col) Origin { get; }

        public AttackRange((int row, int col) origin, Dictionary<(int row, int col), int> locations)
        {
            Origin = origin;
            Locations = locations ?? new Dictionary<(int row, int col), int>();
        }

        public static AttackRange FromNavigationDestinations(NavigationDestinationSet destinationSet, GridSlice area, bool raycastLocations = true)
        {
            (int row, int col) origin = destinationSet.Origin;
            Dictionary<(int row, int col), int> locations = new Dictionary<(int row, int col), int>();
            
            if (raycastLocations)
            {
                foreach (NavigationDestination destination in destinationSet)
                {
                    if (!GridRaycaster.Raycast(origin, destination.Position, destinationSet.SerializedGrid.Obstacles, out _))
                    {
                        (int row, int col) nonNormalizedCoordinates = (destination.Position.ToVector2Int() + area.TopLeft).ToTuple();
                        locations.Add(nonNormalizedCoordinates, destination.StepsRequired);
                    }
                }   
            }

            return new AttackRange(origin, locations);
        }
    }
}