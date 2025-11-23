using System.Collections.Generic;

namespace GridPathFinding
{
    public struct SerializedGrid
    {
        public (int numRows, int numCols) Dimensions { get; }

        public (int row, int col) Origin => _origin ?? (0, 0);

        public (int row, int col) Target => _target ?? (0, 0);
    
        public List<(int x, int y)> Obstacles { get; }
    
        public List<ModificationStep> ModificationSteps { get;  }

        public bool HasOrigin => _origin.HasValue;

        public bool HasTarget => _target.HasValue;

        public bool HasObstacles => Obstacles != null;
    
        public bool HasModificationSteps => ModificationSteps != null;

        private readonly (int row, int col)? _origin;

        private readonly (int row, int col)? _target;

        public SerializedGrid(
            (int numRows, int numCols) dimensions, 
            (int row, int col)? origin, 
            (int row, int col)? target, 
            List<(int x, int y)> obstacles, 
            List<ModificationStep> modificationSteps)
        {
            Dimensions = dimensions;

            _origin = origin;
            _target = target;
        
            Obstacles = obstacles;
            ModificationSteps = modificationSteps;
        }

        public SerializedGrid(char[,] grid)
        {
            Dimensions = (grid.GetLength(0), grid.GetLength(1));
            
            Obstacles = null;
            ModificationSteps = null;

            _origin = null;
            _target = null;
        
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    char point = grid[row, col];
                    
                    if (point == GridPoints.Origin)
                    {
                        _origin = (row, col);
                    }
                    else if (point == GridPoints.Target)
                    {
                        _target = (row, col);
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
    }
}