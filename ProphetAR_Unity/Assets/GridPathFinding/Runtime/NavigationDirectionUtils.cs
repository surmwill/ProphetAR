using System.ComponentModel;

namespace GridPathFinding
{
    public static class NavigationDirectionUtils
    {
        public static (int row, int col) AddInstructionToTuple((int row, int col) coordinates, NavigationInstruction.NavigationDirection navigationDirection)
        {
            switch (navigationDirection)
            {
                case NavigationInstruction.NavigationDirection.Down:
                    return (coordinates.row + 1, coordinates.col);
                
                case NavigationInstruction.NavigationDirection.Up:
                    return (coordinates.row - 1, coordinates.col);
                
                case NavigationInstruction.NavigationDirection.Left:
                    return (coordinates.row, coordinates.col - 1);
                
                case NavigationInstruction.NavigationDirection.Right:
                    return (coordinates.row, coordinates.col + 1);
            }

            throw new InvalidEnumArgumentException();
        }
    }
}