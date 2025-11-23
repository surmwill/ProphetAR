using System;
using System.Collections.Generic;
using System.Linq;

namespace GridPathFinding
{
    public class GridPathFromTester
    {
        const string TestGridsFileName = "TestPathFromGrids.txt";

        public static void TestPathsFrom()
        {
            TestPathsFromWithMaxSteps(3);

            Console.WriteLine("------------ Misc Tests ------------");
            NavigationDestinationSet navigationDestinationSet = GridPathFinder.GetPathsFrom(SerializedGrid.Empty(), 0);
            Console.WriteLine($"Testing if we can move with no max steps (expect false): {navigationDestinationSet.CanMove}\n");
        }

        private static void TestPathsFromWithMaxSteps(int maxNumSteps)
        {
            List<char[,]> grids = GridParser.ParseGridsFromFile(TestGridsFileName, new[] { GridPoints.Target });
        
            for (int i = 0; i < grids.Count; i++)
            {
                char[,] grid = grids[i];
                SerializedGrid serializedGrid = new SerializedGrid(grid);
            
                Console.WriteLine($"------------ Path From - Max Steps ({maxNumSteps}) - Test Grid {i} ------------");

                NavigationDestinationSet navigationDestinationSet = GridPathFinder.GetPathsFrom(serializedGrid, maxNumSteps);
                if (navigationDestinationSet.CanMove)
                {
                    Console.WriteLine("Drawing all paths");
                    GridParser.PrintGrid(DrawDestinationsOnGrid(navigationDestinationSet, grid));
                
                    Console.WriteLine("Verifying data...");
                    ValidateDestinationSet(navigationDestinationSet);
                
                    Console.WriteLine("Drawing furthest path");
                    GridParser.PrintGrid(DrawFurthestDestinationOnGrid(navigationDestinationSet, grid));
                }
                else
                {
                    Console.WriteLine("No valid destinations");
                    Console.WriteLine();
                }
            }
        }

        private static char[,] DrawFurthestDestinationOnGrid(NavigationDestinationSet navigationDestinationSet, char[,] grid)
        {
            NavigationDestination furthestDestination = navigationDestinationSet.ValidDestinations[0];
            foreach (NavigationDestination navigationDestination in navigationDestinationSet.ValidDestinations.Skip(1))
            {
                if (navigationDestination.PathTo.Magnitude > furthestDestination.PathTo.Magnitude)
                {
                    furthestDestination = navigationDestination;
                }
            }

            char[,] gridPath = GridPathToTester.DrawPathOnGrid(furthestDestination.PathTo, GridParser.CopyGrid(grid));

            return gridPath;
        }

        private static char[,] DrawDestinationsOnGrid(NavigationDestinationSet navigationDestinationSet, char[,] grid)
        {
            char[,] gridCopy = GridParser.CopyGrid(grid);
            foreach (NavigationDestination navigationDestination in navigationDestinationSet.ValidDestinations)
            {
                gridCopy[navigationDestination.Position.row, navigationDestination.Position.col] = GridPoints.DEBUG_PRINT_PATH;
            }

            gridCopy[navigationDestinationSet.Origin.row, navigationDestinationSet.Origin.col] = GridPoints.Origin;
            return gridCopy;
        }

        private static bool ValidateDestinationSet(NavigationDestinationSet navigationDestinationSet)
        {
            HashSet<NavigationDestination> validNavigationDestinations = new HashSet<NavigationDestination>(navigationDestinationSet.ValidDestinations);
        
            for (int row = 0; row < navigationDestinationSet.DestinationMap.GetLength(0); row++)
            {
                for (int col = 0; col < navigationDestinationSet.DestinationMap.GetLength(1); col++)
                {
                    NavigationDestination? destination = navigationDestinationSet.DestinationMap[row, col];
                    if (destination.HasValue && !validNavigationDestinations.Remove(destination.Value))
                    {
                        Console.WriteLine($"Expected valid destination at ({row}, {col})\n");
                        return false;
                    }
                }
            }

            if (validNavigationDestinations.Any())
            {
                Console.WriteLine($"Unexpected destinations were reported as valid\n");
                return false;
            }

            Console.WriteLine("Destination set has valid data\n");
            return true;
        }
    }
}