using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace GridOperations.Editor
{
    public class GridRaycastTests
    {
        private static readonly string TestRaycastsFilePath = GridPathTestingUtils.GetTestFilePath("TestGridRaycasts.txt");

        [Test]
        public void TestPrintRaycasts()
        {
            List<char[,]> grids = GridParser.ParseGridsFromFile(TestRaycastsFilePath);

            for (int i = 0; i < grids.Count; i++)
            {
                char[,] grid = grids[i];
                char[,] gridCopy = GridParser.CopyGrid(grid);
                
                SerializedGrid serializedGrid = new SerializedGrid(grid);

                Debug.Log($"------------ Test Grid Raycasts {i} ------------");
                
                Debug.Log("Original grid");
                Debug.Log(serializedGrid.ToString());

                bool hasCollisions = false;
                foreach (((int row, int col) coordinates, char _) in serializedGrid.Where(
                             gridPoint => gridPoint.Value != GridPoints.Obstacle && gridPoint.Value != GridPoints.Origin))
                {
                    if (!GridRaycaster.Raycast(serializedGrid.Origin.Value, coordinates, serializedGrid.Obstacles, out List<(int row, int col)> _))
                    {
                        gridCopy[coordinates.row, coordinates.col] = GridPoints.DEBUG_PRINT_RAY;
                    }
                    else
                    {
                        hasCollisions = true;
                    }
                }


                if (hasCollisions)
                {
                    Debug.Log(GridUtils.PrintRawGrid(gridCopy));
                }
                else
                {
                    Debug.Log("No collisions!");
                }
            }
        }
    }
}