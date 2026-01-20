using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GridPathFinding
{
    public class NavigationDestinationSet : IEnumerable<NavigationDestination>
    {
        public (int row, int col) Origin { get; }
    
        public int MaxNumSteps { get; }

        // Non-null
        public Dictionary<(int row, int col), NavigationDestination> Destinations { get; }

        // Can we move further than the origin?
        public bool CanMove => Destinations.Count > 1 || (Destinations.Count == 1 && Destinations.Keys.First() != Origin);

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

        public IEnumerator<NavigationDestination> GetEnumerator()
        {
            return Destinations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}