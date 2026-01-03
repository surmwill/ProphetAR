using System;
using System.Collections;
using System.Collections.Generic;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Represents a rectangular window of cells relative to the entire grid
    /// </summary>
    public readonly struct GridSlice : IEnumerable<GridCell>
    {
        public CustomGrid Grid { get; }

        public Vector2Int Origin => TopLeft;
        
        public Vector2Int BotLeft { get; }
        
        public Vector2Int TopLeft { get; }
        
        public Vector2 TopRight { get; }
        
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
            BotLeft = topLeft + new Vector2Int(dimensions.x - 1, 0);

            TopRight = topLeft + new Vector2Int(0, dimensions.y - 1);
            BotRight = topLeft + (dimensions - Vector2Int.one);

            Dimensions = dimensions;
        }

        public string SliceDescription()
        {
            return $"Top left/Origin: {TopLeft}, Bot right: {BotRight}, Dimensions: {Dimensions}";
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
                    if (gridCell == null)
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

        public GridCell this[Vector2Int coordinates]
        {
            get
            {
                if (!ContainsPoint(coordinates))
                {
                    throw new ArgumentException($"Given coordinates {coordinates} are not contained within the slice. {SliceDescription()}");
                }
                
                return Grid[coordinates];      
            }
        }

        public IEnumerator<GridCell> GetEnumerator()
        {
            for (int row = TopLeft.y; row <= BotRight.y; row++)
            {
                for (int col = TopLeft.x; col <= BotRight.x; col++)
                {
                    yield return this[(row, col).ToVector2Int()];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public static GridSlice CreateFromCenter(CustomGrid grid, Vector2Int origin, int magnitudeInEachDirection)
        {
            Vector2Int topLeft = origin - magnitudeInEachDirection * Vector2Int.one;
            Vector2Int dimensions = (magnitudeInEachDirection * 2 + 1) * Vector2Int.one;
            return new GridSlice(grid, topLeft, dimensions);
        }
    }
}