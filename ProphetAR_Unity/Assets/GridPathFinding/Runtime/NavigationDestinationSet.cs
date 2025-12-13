using System.Collections.Generic;

namespace GridPathFinding
{
    public class NavigationDestinationSet
    {
        public (int row, int col) Origin { get; }
    
        public int MaxNumSteps { get; }

        public Dictionary<(int row, int col), NavigationDestination> Destinations { get; }

        // The first destination is always the origin (i.e. we don't move)
        public bool CanMove => Destinations.Count > 1;

        public NavigationDestinationSet((int row, int col) origin, int maxNumSteps, Dictionary<(int row, int col), NavigationDestination> destinations)
        {
            Origin = origin;
            MaxNumSteps = maxNumSteps;
            Destinations = destinations;
        }

        public NavigationDestination? this[int row, int col]
        {
            get
            {
                if (Destinations.TryGetValue((row, col), out NavigationDestination destination))
                {
                    return destination;
                }
                
                return null;
            }
        }
    }
}