using System.Collections.Generic;
using System.Linq;
using GridOperations;
using UnityEngine;

namespace ProphetAR
{
    public readonly struct AttackRange
    {
        /// <summary>
        /// Non-null.
        /// The grid cells which are in range of attack.
        /// The keys are their coordinates, and the values are the action points required for attacking that coordinate.
        /// </summary>
        public Dictionary<Vector2Int, int> Locations { get; }

        public Vector2Int Origin { get; }

        public AttackRange(Vector2Int origin, Dictionary<Vector2Int, int> gridLocationsWithActionPoints)
        {
            Origin = origin;
            Locations = gridLocationsWithActionPoints ?? new Dictionary<Vector2Int, int>();
        }
        
        public AttackRange(Vector2Int origin, IEnumerable<Vector2Int> gridLocations, int uniformActionPoints)
        {
            Origin = origin;
            Locations = gridLocations?.ToDictionary(gridLocation => gridLocation, _ => uniformActionPoints) ?? new Dictionary<Vector2Int, int>();
        }

        public static AttackRange FromGridSliceNavigation(GridSliceNavigationDestinations sliceNavigation, int? uniformActionPoints = null, bool raycastCheckLocations = true)
        {
            Dictionary<Vector2Int, int> gridAttackLocations = new Dictionary<Vector2Int, int>();
            
            NavigationDestinationSet destinationSet = sliceNavigation.Destinations;
            foreach (NavigationDestination destination in destinationSet)
            {
                Vector2Int gridCoordinates = sliceNavigation.DestinationSetToGridCoords(destination.Coordinates);
                if (!raycastCheckLocations || !GridRaycaster.Raycast(destination.Origin, destination.Coordinates, destinationSet.SerializedGrid.Obstacles, out _))
                {
                    gridAttackLocations.Add(gridCoordinates, uniformActionPoints ?? destination.StepsRequired);
                }
            }

            Vector2Int gridOrigin = sliceNavigation.DestinationSetToGridCoords(sliceNavigation.Destinations.Origin);
            return new AttackRange(gridOrigin, gridAttackLocations);
        }
    }
}