namespace GridOperations
{
    public static class GridPoints
    {
        // Characters used for designing the map
        public const char Obstacle = '*';
        public const char Origin = 'S';
        public const char Target = 'T';
        public const char Clear = '\0';

        // Characters used while grid solving
        public const char GRID_SOLVING_DIR_BACK_TO_ORIGIN_LEFT = 'L';
        public const char GRID_SOLVING_DIR_BACK_TO_ORIGIN_RIGHT = 'R';
        public const char GRID_SOLVING_DIR_BACK_TO_ORIGIN_UP = 'U';
        public const char GRID_SOLVING_DIR_BACK_TO_ORIGIN_DOWN = 'D';

        // Characters used for debug printing
        public const char DEBUG_PRINT_PATH = '.';
        public const char DEBUG_PRINT_RAY = 'X';
        public const char DEBUG_PRINT_CLEAR = 'O';  // A clear point on the grid is usually the default null character, but this doesn't print well

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
        
        #region GRID_SOLVING
        
        public static bool GridSolvingIsDirectionBackToOrigin(char gridPoint)
        {
            return gridPoint == GRID_SOLVING_DIR_BACK_TO_ORIGIN_RIGHT || 
                   gridPoint == GRID_SOLVING_DIR_BACK_TO_ORIGIN_LEFT || 
                   gridPoint == GRID_SOLVING_DIR_BACK_TO_ORIGIN_UP || 
                   gridPoint == GRID_SOLVING_DIR_BACK_TO_ORIGIN_DOWN;
        }
        
        public static bool GridSolvingCanVisitPoint(char gridPoint)
        {
            return gridPoint != Obstacle && gridPoint != Origin && !GridSolvingIsDirectionBackToOrigin(gridPoint);
        }
        
        #endregion
    }
}