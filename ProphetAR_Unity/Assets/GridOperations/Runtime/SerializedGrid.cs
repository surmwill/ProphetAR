using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GridOperations
{
    public struct SerializedGrid : IEnumerable<KeyValuePair<(int row, int col), char>>
    {
        public (int numRows, int numCols) Dimensions { get; }
        
        public (int row, int col)? Origin { get; set; }

        public (int row, int col)? Target { get; set; }
        
        // Non-null
        public HashSet<(int row, int col)> Obstacles { get; }
        
        // Non-null
        public Dictionary<(int row, int col), ModificationStep> ModificationSteps { get; }

        public SerializedGrid(
            (int numRows, int numCols) dimensions, 
            (int row, int col)? origin = null, 
            (int row, int col)? target = null, 
            IEnumerable<(int row, int col)> obstacles = null, 
            IEnumerable<ModificationStep> modificationSteps = null)
        {
            Dimensions = dimensions;

            Origin = origin;
            Target = target;
        
            Obstacles = new HashSet<(int row, int col)>(obstacles ?? Enumerable.Empty<(int row, int col)>());
            
            ModificationSteps = new Dictionary<(int row, int col), ModificationStep>();
            foreach (ModificationStep modificationStep in modificationSteps ?? Enumerable.Empty<ModificationStep>())
            {
                ModificationSteps.Add(modificationStep.Coordinates, modificationStep);
            }
        }

        public SerializedGrid(char[,] grid)
        {
            Dimensions = (grid.GetLength(0), grid.GetLength(1));
            
            Obstacles = new HashSet<(int row, int col)>();
            ModificationSteps = new Dictionary<(int row, int col), ModificationStep>();

            Origin = null;
            Target = null;
        
            for (int row = 0; row < Dimensions.numRows; row++)
            {
                for (int col = 0; col < Dimensions.numCols; col++)
                {
                    char gridPoint = grid[row, col];
                    
                    switch (gridPoint)
                    {
                        case GridPoints.Origin:
                            Origin = (row, col);
                            break;
                        
                        case GridPoints.Target:
                            Target = (row, col);
                            break;
                        
                        case GridPoints.Obstacle:
                            Obstacles.Add((row, col));
                            break;
                        
                        default:
                        {
                            if (GridPoints.IsModificationStep(gridPoint, out int numSteps))
                            {
                                ModificationStep modificationStep = new ModificationStep((row, col), numSteps);
                                ModificationSteps.Add(modificationStep.Coordinates, modificationStep);
                            }

                            break;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach ((KeyValuePair<(int row, int col), char> gridPoint, int index) in this.Select((gridPoint, index) => (gridPoint, index)))
            {
                sb.Append(gridPoint.Value);
                if ((index + 1) % Dimensions.numCols == 0)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public SerializedGrid WithOrigin((int row, int col) origin)
        {
            ClearCoordinate(origin);
            Origin = origin;
            return this;
        }

        public SerializedGrid WithTarget((int row, int col) target)
        {
            ClearCoordinate(target);
            Target = target;
            return this;
        }

        private void ClearCoordinate((int row, int col) coordinate)
        {
            if (coordinate == Origin)
            {
                Origin = null;
            }
            else if (coordinate == Target)
            {
                Target = null;
            }
            else if (Obstacles.Remove(coordinate))
            {
                // Empty
            }
            else if (ModificationSteps.Remove(coordinate))
            {
                // Empty
            }
        }
        
        public IEnumerator<KeyValuePair<(int row, int col), char>> GetEnumerator()
        {
            for (int row = 0; row < Dimensions.numRows; row++)
            {
                for (int col = 0; col < Dimensions.numCols; col++)
                {
                    (int row, int col) coordinates = (row, col);

                    if (coordinates == Origin)
                    {
                        yield return new KeyValuePair<(int row, int col), char>(coordinates, GridPoints.Origin);
                    }
                    else if (coordinates == Target)
                    {
                        yield return new KeyValuePair<(int row, int col), char>(coordinates, GridPoints.Target);
                    }
                    else if (Obstacles.Contains(coordinates))
                    {
                        yield return new KeyValuePair<(int row, int col), char>(coordinates, GridPoints.Obstacle);
                    }
                    else if (ModificationSteps.TryGetValue(coordinates, out ModificationStep modificationStep))
                    {
                        yield return new KeyValuePair<(int row, int col), char>(coordinates, GridPoints.ModificationStepValueToGridPoint(modificationStep.Value));
                    }
                    else
                    {
                        yield return new KeyValuePair<(int row, int col), char>(coordinates, GridPoints.DEBUG_PRINT_CLEAR);   
                    }
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}