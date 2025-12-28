using System;
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
            if (dimensions.x < 1 || dimensions.y < 1)
            {
                throw new ArgumentException($"Dimensions must be >= 1 {dimensions}");
            }
            
            Grid = grid;

            TopLeft = topLeft;
            BotRight = topLeft + (dimensions - new Vector2Int(1, 1));

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
            List<(int row, int col)> obstacles = new List<(int row, int col)>();
            List<ModificationStep> modificationSteps = new List<ModificationStep>();
            
            for (int row = TopLeft.y; row <= BotRight.y; row++)
            {
                for (int col = TopLeft.x; col <= BotRight.x; col++)
                {
                    GridCell gridCell = Grid[new Vector2Int(row, col)];
                    if (!gridCell)
                    {
                        obstacles.Add((row, col));
                        continue;
                    }
                    
                    GridPointProperties gridPointProperties =  gridCell.GridPointProperties;
                    switch (gridPointProperties.GridPointType)
                    {
                        case GridPointType.Obstacle:
                            obstacles.Add((row, col));
                            break;

                        case GridPointType.ModificationStep:
                            modificationSteps.Add(new ModificationStep((row, col), gridPointProperties.ModificationStep));
                            break;
                    }
                }
            }

            return new SerializedGrid(Dimensions.ToTuple(), obstacles: obstacles, modificationSteps: modificationSteps);
        }
        
        public GridCell this[Vector2Int coordinates] => Grid[coordinates];
    }
}