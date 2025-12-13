using System.Collections.Generic;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Represents a rectangular window of cells relative to the entire grid
    /// </summary>
    public readonly struct GridSlice
    {
        public CustomGrid Grid { get; }
        
        public Vector2Int TopLeft { get; }
        
        public Vector2Int BotRight { get; }
        
        public Vector2Int Dimensions { get; }
        
        /// <summary>
        /// Starting at the top left cell, create a window into the grid (right and down) with the corresponding dimensions,
        /// as if taking a slice of a 2D array
        /// </summary>
        public GridSlice(CustomGrid grid, Vector2Int topLeft, Vector2Int dimensions)
        {
            Grid = grid;

            TopLeft = topLeft;
            BotRight = topLeft + dimensions;

            Dimensions = dimensions;
        }

        public string SliceDescription()
        {
            return $"Top left: {TopLeft}, Bot right: {BotRight}, Dimensions: {Dimensions}";
        }

        public bool ContainsPoint(Vector2Int coordinates)
        {
            return coordinates.x >= TopLeft.x && coordinates.x <= BotRight.x &&
                   coordinates.y >= TopLeft.y && coordinates.y <= BotRight.y;
        }
        
        public SerializedGrid GetSerializedGrid()
        {
            List<(int row, int col)> obstacles = null;
            List<ModificationStep> modificationSteps = null;
            
            for (int row = TopLeft.y; row <= BotRight.y; row++)
            {
                for (int col = TopLeft.x; col <= BotRight.x; col++)
                {
                    GridPointProperties gridPointProperties = Grid[new Vector2Int(row, col)].GridPointProperties;
                    switch (gridPointProperties.GridPointType)
                    {
                        case GridPointType.Obstacle:
                            (obstacles ??= new List<(int row, int col)>()).Add((row, col));
                            break;

                        case GridPointType.ModificationStep:
                            (modificationSteps ??= new List<ModificationStep>()).Add(new ModificationStep((row, col), gridPointProperties.ModificationStep));
                            break;
                    }
                }
            }

            return new SerializedGrid(Dimensions.ToTuple(), obstacles: obstacles, modificationSteps: modificationSteps);
        }
        
        public GridCell this[Vector2Int coordinates] => Grid[coordinates];
    }
}