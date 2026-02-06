using System.Collections.Generic;
using System.Text;

namespace GridOperations
{
    public struct SerializedGrid
    {
        public (int numRows, int numCols) Dimensions { get; }
        
        public (int row, int col)? Origin { get; set; }

        public (int row, int col)? Target { get; set; }
        
        // Non-null
        public List<(int row, int col)> Obstacles { get; set; }
        
        // Non-null
        public List<ModificationStep> ModificationSteps { get; set; }

        public SerializedGrid(
            (int numRows, int numCols) dimensions, 
            (int row, int col)? origin = null, 
            (int row, int col)? target = null, 
            List<(int row, int col)> obstacles = null, 
            List<ModificationStep> modificationSteps = null)
        {
            Dimensions = dimensions;

            Origin = origin;
            Target = target;
        
            Obstacles = obstacles ?? new List<(int row, int col)>();
            ModificationSteps = modificationSteps ?? new List<ModificationStep>();
        }

        public SerializedGrid(char[,] grid)
        {
            Dimensions = (grid.GetLength(0), grid.GetLength(1));
            
            Obstacles = new List<(int row, int col)>();
            ModificationSteps = new List<ModificationStep>();

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
                                ModificationSteps.Add(new ModificationStep((row, col), numSteps));
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
            
            for (int row = 0; row < Dimensions.numRows; row++)
            {
                for (int col = 0; col < Dimensions.numCols; col++)
                {
                    (int row, int col) coordinates = (row, col);

                    if (coordinates == Origin)
                    {
                        sb.Append(GridPoints.Origin);
                        continue;
                    }
                    
                    if (coordinates == Target)
                    {
                        sb.Append(GridPoints.Target);
                        continue;
                    }
                    
                    if (Obstacles?.Contains(coordinates) ?? false)
                    {
                        sb.Append(GridPoints.Obstacle);
                        continue;
                    }
                    
                    int modificationStepIndex = ModificationSteps?.FindIndex(modificationStep => coordinates == modificationStep.Coordinates) ?? -1;
                    if (modificationStepIndex >= 0)
                    {
                        sb.Append(GridPoints.ModificationStepValueToGridPoint(ModificationSteps[modificationStepIndex].Value));
                        continue;
                    }

                    sb.Append(GridPoints.DEBUG_PRINT_CLEAR);
                }

                sb.AppendLine();
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
            if (Obstacles.Remove(coordinate))
            {
                return;
            }
            
            int modificationStepIndex = ModificationSteps.FindIndex(modificationStep => coordinate == modificationStep.Coordinates);
            if (modificationStepIndex >= 0)
            {
                ModificationSteps.RemoveAt(modificationStepIndex);
                return;
            }
        }
    }
}