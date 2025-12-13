namespace GridPathFinding
{
    public static class GridPoints
    {
        public const char Obstacle = '*';
        public const char Origin = 'S';
        public const char Target = 'T';
        public const char Clear = '\0';

        public const char DIR_BACK_TO_ORIGIN_LEFT = 'L';
        public const char DIR_BACK_TO_ORIGIN_RIGHT = 'R';
        public const char DIR_BACK_TO_ORIGIN_UP = 'U';
        public const char DIR_BACK_TO_ORIGIN_DOWN = 'B';

        public const char DEBUG_PRINT_PATH = '.';
    
        // Because null doesn't print (note: O and not zero. Zero (0) is a number and a modification step)
        public const char DEBUG_PRINT_CLEAR = 'O';

        public static bool IsDirectionBackToOrigin(char gridPoint)
        {
            return gridPoint == DIR_BACK_TO_ORIGIN_RIGHT || 
                   gridPoint == DIR_BACK_TO_ORIGIN_LEFT || 
                   gridPoint == DIR_BACK_TO_ORIGIN_UP || 
                   gridPoint == DIR_BACK_TO_ORIGIN_DOWN;
        }

        public static bool IsModificationStep(char gridPoint, out int modifiedNumSteps)
        {
            modifiedNumSteps = 0;
        
            if (char.IsDigit(gridPoint))
            {
                modifiedNumSteps = gridPoint - '0';
                return true;
            }

            return false;
        }

        public static char ModificationStepValueToGridPoint(int modificationStepValue)
        {
            return (char) ('0' + modificationStepValue);
        }
    
        public static bool IsOccupiedPoint(char gridPoint)
        {
            return gridPoint == Obstacle || gridPoint == Origin || IsDirectionBackToOrigin(gridPoint);
        }
    
    }
}