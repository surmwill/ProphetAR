using System.IO;

namespace GridOperations
{
    public struct NavigationDestination
    {
        public (int row, int col) Coordinates { get; }
    
        public (int row, int col) Origin { get; }
    
        public int StepsRequired { get; }
    
        private NavigationInstructionSet _pathTo;
    
        private readonly char[,] _solvedGrid;

        public NavigationInstructionSet PathTo
        {
            get
            {
                if (_pathTo == null)
                {
                    NavigationInstructionSet pathToOrigin = GridPathFinder.ReverseBuildPathToOrigin(Coordinates, _solvedGrid);
                    _pathTo = pathToOrigin ?? throw new InvalidDataException("A navigation destination should not exist if it cannot be reached from its origin");
                }

                return _pathTo;
            }
        }

        public NavigationDestination((int row, int col) coordinates, (int row, int col) origin, int stepsRequired, char[,] solvedGrid)
        {
            Coordinates = coordinates;
            Origin = origin;
        
            StepsRequired = stepsRequired;
        
            _solvedGrid = solvedGrid;
            _pathTo = null;
        }
    }
}