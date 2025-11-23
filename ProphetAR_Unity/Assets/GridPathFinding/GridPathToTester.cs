using System;
using System.Collections.Generic;
using static GridPathFinding.NavigationInstruction;

namespace GridPathFinding
{
    public static class GridPathToTester
    {
        const string TestGridsFileName = "TestPathToGrids.txt";

        public static void TestPathsTo()
        {
            TestPathingOnGrid();

            Console.WriteLine("------------ Misc Tests ------------");
            SerializedGrid outOfBoundsGrid = new SerializedGrid((1, 1), (0, 0), (99, 99), null, null);
            NavigationInstructionSet navigationInstructionSet = GridPathFinder.GetPathTo(outOfBoundsGrid);
            Console.WriteLine($"Testing we receive null instructions if the target is out of bounds. Instructions are null: {navigationInstructionSet == null}");
        }

        private static void TestPathingOnGrid()
        {
            List<char[,]> grids = GridParser.ParseGridsFromFile(TestGridsFileName);

            for (int i = 0; i < grids.Count; i++)
            {
                char[,] grid = grids[i];
                SerializedGrid serializedGrid = new SerializedGrid(grid);
            
                Console.WriteLine($"------------ Path To - Test Grid {i} ------------");
            
                NavigationInstructionSet instructionSet = GridPathFinder.GetPathTo(serializedGrid);
                if (instructionSet != null)
                {
                    if (instructionSet.Origin == instructionSet.Target)
                    {
                        Console.WriteLine("No path: origin is the same as target");
                    }
                    else
                    {
                        GridParser.PrintGrid(DrawPathOnGrid(instructionSet, grid));
                        instructionSet.PrintInstructions();
                    }
                }
                else
                {
                    Console.WriteLine("No path found");
                    Console.WriteLine();
                }
            }
        }

        public static char[,] DrawPathOnGrid(NavigationInstructionSet instructionSet, char[,] grid)
        {
            char[,] gridCopy = GridParser.CopyGrid(grid);
        
            (int row, int col) currentPosition = instructionSet.Origin;
            foreach (NavigationInstruction navigationInstruction in instructionSet.PathToTarget)
            {
                (int moveCols, int moveRows) = (0, 0);

                switch (navigationInstruction.Direction)
                {
                    case NavigationDirection.Left:
                        moveCols = navigationInstruction.Magnitude * -1;
                        break;
                
                    case NavigationDirection.Right:
                        moveCols = navigationInstruction.Magnitude;
                        break;
                
                    case NavigationDirection.Up:
                        moveRows = navigationInstruction.Magnitude * -1;
                        break;
                
                    case NavigationDirection.Down:
                        moveRows = navigationInstruction.Magnitude;
                        break;
                }

                for (int i = 0; i < Math.Abs(moveCols); i++)
                {
                    currentPosition.col += moveCols > 0 ? 1 : -1;
                    gridCopy[currentPosition.row, currentPosition.col] = GridPoints.DEBUG_PRINT_PATH;
                }
            
                for (int i = 0; i < Math.Abs(moveRows); i++)
                {
                    currentPosition.row += moveRows > 0 ? 1 : -1;
                    gridCopy[currentPosition.row, currentPosition.col] = GridPoints.DEBUG_PRINT_PATH;
                }
            }
        
            gridCopy[instructionSet.Target.row, instructionSet.Target.col] = GridPoints.Target;
            return gridCopy;
        }
    }
}