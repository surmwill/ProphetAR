using System.Collections.Generic;

namespace GridPathFinding
{
    public struct SerializedGrid
    {
        public (int numRows, int numCols) Dimensions { get; }
        
        public (int row, int col)? Origin { get; set; }

        public (int row, int col)? Target { get; set; }

        public List<(int x, int y)> Obstacles { get; set; }
    
        public List<ModificationStep> ModificationSteps { get; set; }

        public bool HasObstacles => Obstacles != null;
    
        public bool HasModificationSteps => ModificationSteps != null;

        public SerializedGrid(
            (int numRows, int numCols) dimensions, 
            (int row, int col)? origin = null, 
            (int row, int col)? target = null, 
            List<(int x, int y)> obstacles = null, 
            List<ModificationStep> modificationSteps = null)
        {
            Dimensions = dimensions;

            Origin = origin;
            Target = target;
        
            Obstacles = obstacles;
            ModificationSteps = modificationSteps;
        }

        public SerializedGrid(char[,] grid)
        {
            Dimensions = (grid.GetLength(0), grid.GetLength(1));
            
            Obstacles = null;
            ModificationSteps = null;

            Origin = null;
            Target = null;
        
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    char point = grid[row, col];
                    
                    if (point == GridPoints.Origin)
                    {
                        Origin = (row, col);
                    }
                    else if (point == GridPoints.Target)
                    {
                        Target = (row, col);
                    }
                    else if (point == GridPoints.Obstacle)
                    {
                        (Obstacles ??= new List<(int x, int y)>()).Add((row, col));
                    }
                    else if (GridPoints.IsModificationStep(point, out int numSteps))
                    {
                        (ModificationSteps ??= new List<ModificationStep>()).Add(new ModificationStep((row, col), numSteps));
                    }
                }
            }
        }

        public SerializedGrid WithOrigin((int row, int col) origin)
        {
            Origin = origin;
            return this;
        }

        public SerializedGrid WithTarget((int row, int col) target)
        {
            Target = target;
            return this;
        }
    }
}