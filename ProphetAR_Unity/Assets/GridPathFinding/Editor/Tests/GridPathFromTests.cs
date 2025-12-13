using System.Collections.Generic;
using System.Linq;
using GridPathPathFinding.Editor;
using NUnit.Framework;
using UnityEngine;

namespace GridPathFinding.Editor
{
    public class GridPathFromTests
    {
        private static readonly string TestGridsFileName = GridPathTestingUtils.GetTestFilePath("TestPathFromGrids.txt");

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void TestPathsWithMaxSteps(int maxSteps)
        {
            TestPathsFromWithMaxSteps(TestGridsFileName, maxSteps);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void TestZeroOrNegativeMaxSteps(int zeroOrNegativeSteps)
        {
            char[,] gridRaw = new char[1, 1];
            gridRaw[0, 0] = GridPoints.Origin;
            SerializedGrid grid = new SerializedGrid(gridRaw);
            
            NavigationDestinationSet navigationDestinationSet = GridPathFinder.GetPathsFrom(grid, 0);
            Assert.IsFalse(navigationDestinationSet.CanMove, "We should not be able to move with zero or negative max steps");
        }

        private static void TestPathsFromWithMaxSteps(string inputFilePath, int maxNumSteps)
        {
            List<char[,]> grids = GridParser.ParseGridsFromFile(inputFilePath, new[] { GridPoints.Target });

            for (int i = 0; i < grids.Count; i++)
            {
                char[,] grid = grids[i];
                SerializedGrid serializedGrid = new SerializedGrid(grid);

                Debug.Log($"------------ Path From - Max Steps ({maxNumSteps}) - Test Grid {i} ------------");

                NavigationDestinationSet navigationDestinationSet = GridPathFinder.GetPathsFrom(serializedGrid, maxNumSteps);
                if (navigationDestinationSet.CanMove)
                {
                    Debug.Log("Original grid");
                    Debug.Log(GridParser.ShowGrid(grid));
                    
                    Debug.Log("Drawing all paths");
                    Debug.Log(GridParser.ShowGrid(DrawDestinationsOnGrid(navigationDestinationSet, grid)));

                    Debug.Log("Drawing furthest path");
                    Debug.Log(GridParser.ShowGrid(DrawFurthestDestinationOnGrid(navigationDestinationSet, grid)));
                }
                else
                {
                    Debug.Log("No valid destinations");
                }
            }
        }

        private static char[,] DrawFurthestDestinationOnGrid(NavigationDestinationSet navigationDestinationSet, char[,] grid)
        {
            NavigationDestination furthestDestination = navigationDestinationSet.Destinations.Values.First();
            foreach (NavigationDestination navigationDestination in navigationDestinationSet.Destinations.Values.Skip(1))
            {
                if (navigationDestination.PathTo.Magnitude > furthestDestination.PathTo.Magnitude)
                {
                    furthestDestination = navigationDestination;
                }
            }

            char[,] gridPath = GridPathToTests.DrawPathOnGrid(furthestDestination.PathTo, GridParser.CopyGrid(grid));
            return gridPath;
        }

        private static char[,] DrawDestinationsOnGrid(NavigationDestinationSet navigationDestinationSet, char[,] grid)
        {
            char[,] gridCopy = GridParser.CopyGrid(grid);
            foreach (NavigationDestination navigationDestination in navigationDestinationSet.Destinations.Values)
            {
                gridCopy[navigationDestination.Position.row, navigationDestination.Position.col] = GridPoints.DEBUG_PRINT_PATH;
            }

            gridCopy[navigationDestinationSet.Origin.row, navigationDestinationSet.Origin.col] = GridPoints.Origin;
            return gridCopy;
        }
    }
}
