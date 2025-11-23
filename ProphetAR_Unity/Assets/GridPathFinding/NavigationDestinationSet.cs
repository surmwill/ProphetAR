using System.Collections.Generic;

namespace GridPathFinding
{
    public class NavigationDestinationSet
    {
        public (int row, int col) Origin { get; }
    
        public int MaxNumSteps { get; }
    
        public NavigationDestination?[,] DestinationMap;
    
        public List<NavigationDestination> ValidDestinations { get; }

        // The first destination is always the origin (i.e. we don't move)
        public bool CanMove => ValidDestinations.Count > 1;

        public NavigationDestinationSet((int row, int col) origin, int maxNumSteps, NavigationDestination?[,] destinationMap, List<NavigationDestination> validDestinations)
        {
            Origin = origin;
            MaxNumSteps = maxNumSteps;
        
            DestinationMap = destinationMap;
            ValidDestinations = validDestinations;
        }
    }
}