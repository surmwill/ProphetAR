using System;
using System.Collections.Generic;
using GridOperations;
using GridOperations.Editor;
using NUnit.Framework;
using UnityEngine;

namespace GridOperations.Editor
{
    public class GridPathToTests
    {
        private static readonly string TestGridsFilePath = GridPathTestingUtils.GetTestFilePath("TestPathToGrids.txt");

        [Test]
        public static void TestOutOfBounds()
        {
            SerializedGrid outOfBoundsGrid = new SerializedGrid((1, 1), (0, 0), (99, 99), null, null);
            NavigationInstructionSet navigationInstructionSet = GridPathFinder.GetPathTo(outOfBoundsGrid);
            Assert.IsTrue(navigationInstructionSet == null, "Target is out of bounds. There should be no instructions.");
        }

        [Test]
        public static void TestFindingPaths()
        {
            List<char[,]> grids = GridParser.ParseGridsFromFile(TestGridsFilePath);

            for (int i = 0; i < grids.Count; i++)
            {
                char[,] grid = grids[i];
                SerializedGrid serializedGrid = new SerializedGrid(grid);
            
                Debug.Log($"------------ Path To - Test Grid {i} ------------");
                
                NavigationInstructionSet instructionSet = GridPathFinder.GetPathTo(serializedGrid);
                if (instructionSet != null)
                {
                    if (instructionSet.Origin == instructionSet.Target)
                    {
                        Assert.IsTrue(instructionSet.PathToTarget.Count == 0, "Origin is the same as target. We don't need to move");
                        Assert.IsTrue(instructionSet.PathCoordinates[0] == instructionSet.Origin);
                    }
                    else
                    {
                        char[,] gridWithPath = DrawPathOnGrid(instructionSet, grid);
                        
                        Debug.Log(GridPathFinder.PrintGrid(gridWithPath));
                        Debug.Log(instructionSet.ListInstructionsString());
                        Debug.Log(instructionSet.ListPathCoordinatesString());
                        
                        VerifyPathCoordinatesWithGrid(gridWithPath, instructionSet);
                    }
                }
                else
                {
                    Debug.Log("No path found");
                }
            }
        }

        private static void VerifyPathCoordinatesWithGrid(char[,] gridWithPath, NavigationInstructionSet path)
        {
            foreach ((int row, int col) pathCoordinate in path.PathCoordinates)
            {
                if (pathCoordinate == path.Origin)
                {
                    Assert.IsTrue(gridWithPath[pathCoordinate.row, pathCoordinate.col] == GridPoints.Origin);
                }
                else if (pathCoordinate == path.Target)
                {
                    Assert.IsTrue(gridWithPath[pathCoordinate.row, pathCoordinate.col] == GridPoints.Target);
                }
                else
                {
                    Assert.IsTrue(gridWithPath[pathCoordinate.row, pathCoordinate.col] == GridPoints.DEBUG_PRINT_PATH);
                }
            }

            for (int i = 0; i < path.PathCoordinates.Count; i++)
            {
                Assert.IsTrue(path.GetStepNumberForCoordinate(path.PathCoordinates[i]) == i);
            }
        }

        public static char[,] DrawPathOnGrid(NavigationInstructionSet instructionSet, char[,] grid)
        {
            char[,] gridCopy = GridParser.CopyGrid(grid);
        
            (int row, int col) currentPosition = instructionSet.Origin;
            gridCopy[currentPosition.row, currentPosition.col] = GridPoints.Origin;
            
            foreach (NavigationInstruction navigationInstruction in instructionSet.PathToTarget)
            {
                (int moveCols, int moveRows) = (0, 0);

                switch (navigationInstruction.Direction)
                {
                    case NavigationInstruction.NavigationDirection.Left:
                        moveCols = navigationInstruction.Magnitude * -1;
                        break;
                
                    case NavigationInstruction.NavigationDirection.Right:
                        moveCols = navigationInstruction.Magnitude;
                        break;
                
                    case NavigationInstruction.NavigationDirection.Up:
                        moveRows = navigationInstruction.Magnitude * -1;
                        break;
                
                    case NavigationInstruction.NavigationDirection.Down:
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
