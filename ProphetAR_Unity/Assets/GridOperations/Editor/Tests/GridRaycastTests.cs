using System.Collections.Generic;
using UnityEngine;

namespace GridOperations.Editor
{
    public class GridRaycastTests
    {
        private static readonly string TestRaycastsFilePath = GridPathTestingUtils.GetTestFilePath("TestGridRaycasts.txt");

        public void TestRaycasts()
        {
            List<char[,]> grids = GridParser.ParseGridsFromFile(TestRaycastsFilePath);

            for (int i = 0; i < grids.Count; i++)
            {
                char[,] grid = grids[i];
                SerializedGrid serializedGrid = new SerializedGrid(grid);

                Debug.Log($"------------ Test Grid Raycasts {i} ------------");
                
                Debug.Log("Original grid");
                Debug.Log(GridUtils.PrintRawGrid(grid));

                foreach (var VARIABLE in serializedGrid.)
                {
                    
                }
                
                GridRaycaster.Raycast(serializedGrid.Origin, )
                
                

                NavigationDestinationSet navigationDestinationSet = GridPathFinder.GetPathsFrom(serializedGrid, maxNumSteps);
                if (navigationDestinationSet.CanMove)
                {
                    Debug.Log("Original grid");
                    Debug.Log(GridPathFinder.PrintGrid(grid));
                    
                    Debug.Log("Drawing all paths");
                    Debug.Log(GridPathFinder.PrintGrid(DrawDestinationsOnGrid(navigationDestinationSet, grid)));

                    Debug.Log("Drawing furthest path");
                    Debug.Log(GridPathFinder.PrintGrid(DrawFurthestDestinationOnGrid(navigationDestinationSet, grid)));
                }
                else
                {
                    Debug.Log("No valid destinations");
                }
            }
        }
    }
}