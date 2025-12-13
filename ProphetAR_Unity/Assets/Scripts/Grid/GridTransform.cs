using System;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Anything that needs to be located on the grid has this
    /// </summary>
    public class GridTransform
    {
        public CustomGrid Grid { get; private set; }
        
        public Vector2Int Coordinates { get; private set; }

        public GridTransform(CustomGrid grid, Vector2Int coordinates)
        {
            Grid = grid;
            Coordinates = coordinates;
        }

        public NavigationDestinationSet GetPathsFrom(int maxNumSteps, GridSlice area)
        {
            if (!area.ContainsPoint(Coordinates))
            {
                throw new ArgumentException($"The transform at {Coordinates} is not contained in the grid slice with properties: (${area.SliceDescription()})");
            }

            SerializedGrid serializedGrid = area.GetSerializedGrid().WithOrigin(Coordinates.ToTuple());
            return GridPathFinder.GetPathsFrom(serializedGrid, maxNumSteps);
        }
        
        public NavigationInstructionSet GetPathTo(Vector2Int target, GridSlice area)
        {
            if (!area.ContainsPoint(Coordinates))
            {
                throw new ArgumentException($"The transform at {Coordinates} is not contained in the grid slice with properties: (${area.SliceDescription()})");
            }
            
            if (!area.ContainsPoint(target))
            {
                throw new ArgumentException($"The target {target} is not contained in the grid slice with properties: (${area.SliceDescription()})");
            }

            SerializedGrid serializedGrid = area.GetSerializedGrid()
                .WithOrigin(Coordinates.ToTuple())
                .WithTarget(target.ToTuple());

            return GridPathFinder.GetPathTo(serializedGrid);
        }
    }
}