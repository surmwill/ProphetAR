using System;
using GridOperations;
using UnityEngine;

namespace ProphetAR
{
    public class GridPainter
    {
        public CustomGrid Grid { get; }
        
        public GridPainter(CustomGrid grid)
        {
            Grid = grid;
        }
        
        // Movement
        public DisposableGridPainter ShowMovementArea(GridSliceNavigationDestinationSet sliceDestinations)
        {
            NavigationDestinationSet destinationSet = sliceDestinations.DestinationSet;
            foreach (GridCell gridCell in sliceDestinations.Slice)
            {
                (int row, int col) destinationSetCoords = sliceDestinations.GridToDestinationSetCoords(gridCell.Coordinates);
                if (destinationSet.Destinations.TryGetValue(destinationSetCoords, out NavigationDestination destination))
                {
                    gridCell.GridCellPainter.ShowIsNavigable(true, destination.StepsRequired);
                }
            }

            return new DisposableGridPainter(() => ClearMovementArea(sliceDestinations.Slice));
        }
        
        public void ClearMovementArea(GridSlice gridSlice)
        {
            foreach (GridCell gridCell in gridSlice)
            {
                gridCell.GridCellPainter.ShowIsNavigable(false);
            }
        }

        // Attack
        public DisposableGridPainter ShowAttackableArea(AttackRange attackRange)
        {
            foreach ((Vector2Int coordinates, int actionPoints) in attackRange.Locations)
            {
                Grid[coordinates].GridCellPainter.ShowIsAttackable(true, actionPoints);
            }

            return new DisposableGridPainter(() => ClearAttackableArea(attackRange));
        }
        
        public void ClearAttackableArea(AttackRange attackRange)
        {
            foreach (Vector2Int coordinates in attackRange.Locations.Keys)
            {
                Grid[coordinates].GridCellPainter.ShowIsAttackable(false);
            }
        }

        /// <summary>
        /// Allows us to paint grid cells in using blocks { } instead of having paired ShowPaint() HidePaint() calls.
        /// This is optional and the ShowPaint() HidePaint() pattern can be used all the same.
        /// </summary>
        public class DisposableGridPainter : IDisposable
        {
            private readonly Action _cleanup;
            
            public DisposableGridPainter(Action cleanup)
            {
                _cleanup = cleanup;
            }

            public void Dispose()
            {
                _cleanup?.Invoke();
            }
        }
    }
}