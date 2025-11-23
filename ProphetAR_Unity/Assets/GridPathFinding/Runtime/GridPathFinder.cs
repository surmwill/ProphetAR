using System;
using System.Collections.Generic;
using System.Linq;

namespace GridPathFinding
{
    public static class GridPathFinder
    {
        private static readonly Dictionary<(int numRows, int numCols), char[,]> GridCache = new();

        private static char[,] CreateOrClearGrid(int numRows, int numCols)
        {
            if (!GridCache.TryGetValue((numRows, numCols), out char[,] grid))
            {
                grid = new char[numRows, numCols];
                GridCache[(numRows, numCols)] = grid;
            }
            else
            {
                Array.Clear(grid, 0, grid.Length);
            }
        
            return grid;
        }
    
        private static bool IsPointInGrid((int row, int col) point, (int numRows, int numCols) dimensions)
        {
            return point.row >= 0 && point.row < dimensions.numRows && point.col >= 0 && point.col < dimensions.numCols;
        }

        private static void AddSpecialGridPoints(char[,] grid, SerializedGrid serializedGrid)
        {
            (int numRows, int numCols) gridDimensions = (grid.GetLength(0), grid.GetLength(1));

            if (serializedGrid.HasOrigin)
            {
                grid[serializedGrid.Origin.row, serializedGrid.Origin.col] = GridPoints.Origin;
            }

            if (serializedGrid.HasTarget)
            {
                grid[serializedGrid.Target.row, serializedGrid.Target.col] = GridPoints.Target;
            }

            if (serializedGrid.HasObstacles)
            {
                foreach ((int row, int col) in serializedGrid.Obstacles.Where(obstacle => IsPointInGrid(obstacle, gridDimensions)))
                {
                    grid[row, col] = GridPoints.Obstacle;
                }   
            }

            if (serializedGrid.HasModificationSteps)
            {
                foreach (ModificationStep modificationStep in serializedGrid.ModificationSteps.Where(modificationStep => IsPointInGrid(modificationStep.Position, gridDimensions)))
                {
                    grid[modificationStep.Position.row, modificationStep.Position.col] = GridPoints.ModificationStepValueToGridPoint(modificationStep.Value);
                }   
            }
        }

        private static char[,] DeserializeGrid(SerializedGrid serializedGrid)
        {
            char[,] grid = CreateOrClearGrid(serializedGrid.Dimensions.numRows, serializedGrid.Dimensions.numCols);
            AddSpecialGridPoints(grid, serializedGrid);
            return grid;
        }

        public static NavigationDestinationSet GetPathsFrom(SerializedGrid serializedGrid, int maxNumSteps)
        {
            if (!serializedGrid.HasOrigin)
            {
                throw new ArgumentException("No origin");
            }

            if (!IsPointInGrid(serializedGrid.Origin, serializedGrid.Dimensions))
            {
                throw new ArgumentException("Origin is not in the grid");
            }

            maxNumSteps = Math.Max(maxNumSteps, 0);
            char[,] solvedGrid = DeserializeGrid(serializedGrid);
        
            NavigationDestination?[,] navigationDestinations = new NavigationDestination?[serializedGrid.Dimensions.numRows, serializedGrid.Dimensions.numCols];
            List<NavigationDestination> validNavigationDestinations = new List<NavigationDestination>();
        
            NavigationDestination navigationDestination = new NavigationDestination(serializedGrid.Origin, serializedGrid.Origin, 0, solvedGrid);
            navigationDestinations[serializedGrid.Origin.row, serializedGrid.Origin.col] = navigationDestination;
            validNavigationDestinations.Add(navigationDestination);
        
            Queue<(int row, int col, int numSteps)> nextPositions = new Queue<(int row, int col, int numSteps)>();
            nextPositions.Enqueue((serializedGrid.Origin.row, serializedGrid.Origin.col, 0));
        
            while (nextPositions.Count > 0)
            {
                (int row, int col, int numSteps) currentPosition = nextPositions.Dequeue();
            
                if (currentPosition.col > 0 && !GridPoints.IsOccupiedPoint(solvedGrid[currentPosition.row, currentPosition.col - 1]))
                {
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row, currentPosition.col - 1], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row, currentPosition.col - 1] = GridPoints.DIR_BACK_TO_ORIGIN_RIGHT;
                        navigationDestination = new NavigationDestination((currentPosition.row, currentPosition.col - 1), serializedGrid.Origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[currentPosition.row, currentPosition.col - 1] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row, currentPosition.col - 1, currentPosition.numSteps + modifiedNumSteps));  
                        validNavigationDestinations.Add(navigationDestination);
                    }
                }

                if (currentPosition.row < serializedGrid.Dimensions.numRows - 1 && !GridPoints.IsOccupiedPoint(solvedGrid[currentPosition.row + 1, currentPosition.col]))
                { 
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row + 1, currentPosition.col], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row + 1, currentPosition.col] = GridPoints.DIR_BACK_TO_ORIGIN_UP;
                        navigationDestination = new NavigationDestination((currentPosition.row + 1, currentPosition.col), serializedGrid.Origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[currentPosition.row + 1, currentPosition.col] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row + 1, currentPosition.col, currentPosition.numSteps + modifiedNumSteps));  
                        validNavigationDestinations.Add(navigationDestination);   
                    }
                }

                if (currentPosition.col < serializedGrid.Dimensions.numCols - 1 && !GridPoints.IsOccupiedPoint(solvedGrid[currentPosition.row, currentPosition.col + 1]))
                {
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row, currentPosition.col + 1], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row, currentPosition.col + 1] = GridPoints.DIR_BACK_TO_ORIGIN_LEFT;
                        navigationDestination = new NavigationDestination((currentPosition.row, currentPosition.col + 1), serializedGrid.Origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[currentPosition.row, currentPosition.col + 1] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row, currentPosition.col + 1, currentPosition.numSteps + modifiedNumSteps));
                        validNavigationDestinations.Add(navigationDestination);
                    }
                }

                if (currentPosition.row > 0 && !GridPoints.IsOccupiedPoint(solvedGrid[currentPosition.row - 1, currentPosition.col]))
                {
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row - 1, currentPosition.col], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row - 1, currentPosition.col] = GridPoints.DIR_BACK_TO_ORIGIN_DOWN;
                        navigationDestination = new NavigationDestination((currentPosition.row - 1, currentPosition.col), serializedGrid.Origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[currentPosition.row - 1, currentPosition.col] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row - 1, currentPosition.col, currentPosition.numSteps + modifiedNumSteps));
                        validNavigationDestinations.Add(navigationDestination);
                    }
                }
            }

            return new NavigationDestinationSet(serializedGrid.Origin, maxNumSteps, navigationDestinations, validNavigationDestinations);
        }

        public static NavigationInstructionSet GetPathTo(SerializedGrid serializedGrid)
        {
            if (!serializedGrid.HasOrigin)
            {
                throw new ArgumentException("No origin");
            }

            if (!serializedGrid.HasTarget)
            {
                throw new ArgumentException("No target");
            }

            if (!IsPointInGrid(serializedGrid.Origin, serializedGrid.Dimensions))
            {
                throw new ArgumentException("Origin is not in the grid");
            }
        
            if (!IsPointInGrid(serializedGrid.Target, serializedGrid.Dimensions))
            {
                return null;
            }
        
            if (serializedGrid.Origin == serializedGrid.Target)
            {
                return new NavigationInstructionSet(serializedGrid.Origin, serializedGrid.Target, new List<NavigationInstruction>());
            }

            char[,] solvedGrid = DeserializeGrid(serializedGrid);
        
            Queue<(int row, int col)> nextPositions = new Queue<(int row, int col)>();
            nextPositions.Enqueue(serializedGrid.Origin);
        
            bool found = false;
            while (nextPositions.Count > 0)
            {
                (int row, int col) currentPosition = nextPositions.Dequeue();
                if (currentPosition == serializedGrid.Target)
                {
                    found = true;
                    break;
                }
            
                if (currentPosition.col > 0 && !GridPoints.IsOccupiedPoint(solvedGrid[currentPosition.row, currentPosition.col - 1]))
                {
                    solvedGrid[currentPosition.row, currentPosition.col - 1] = GridPoints.DIR_BACK_TO_ORIGIN_RIGHT;
                    nextPositions.Enqueue((currentPosition.row, currentPosition.col - 1));   
                }

                if (currentPosition.row < serializedGrid.Dimensions.numRows - 1 && !GridPoints.IsOccupiedPoint(solvedGrid[currentPosition.row + 1, currentPosition.col]))
                {
                    solvedGrid[currentPosition.row + 1, currentPosition.col] = GridPoints.DIR_BACK_TO_ORIGIN_UP;
                    nextPositions.Enqueue((currentPosition.row + 1, currentPosition.col));   
                }

                if (currentPosition.col < serializedGrid.Dimensions.numCols - 1 && !GridPoints.IsOccupiedPoint((solvedGrid[currentPosition.row, currentPosition.col + 1])))
                {
                    solvedGrid[currentPosition.row, currentPosition.col + 1] = GridPoints.DIR_BACK_TO_ORIGIN_LEFT;
                    nextPositions.Enqueue((currentPosition.row, currentPosition.col + 1));   
                }

                if (currentPosition.row > 0 && !GridPoints.IsOccupiedPoint(solvedGrid[currentPosition.row - 1, currentPosition.col]))
                {
                    solvedGrid[currentPosition.row - 1, currentPosition.col] = GridPoints.DIR_BACK_TO_ORIGIN_DOWN;
                    nextPositions.Enqueue((currentPosition.row - 1, currentPosition.col));   
                }
            }

            return found ? ReverseBuildPathToOrigin(serializedGrid.Target, solvedGrid) : null;
        }

        public static NavigationInstructionSet ReverseBuildPathToOrigin((int row, int col) currentPosition, char[,] solvedGrid)
        {
            int totalMagnitude = 0;
            List<NavigationInstruction> navigationInstructions = new List<NavigationInstruction>();
            (int row, int col) foundOrigin = default;
        
            ReverseAndRememberDirectionBack(currentPosition, navigationInstructions, ref totalMagnitude, ref foundOrigin, solvedGrid);
            return new NavigationInstructionSet(foundOrigin, currentPosition, navigationInstructions, totalMagnitude);
        }

        private static void ReverseAndRememberDirectionBack(
            (int row, int col) currentPosition, 
            List<NavigationInstruction> originToTarget, 
            ref int totalMagnitude,
            ref (int row, int col) foundOrigin,
            char[,] solvedGrid)
        {
            totalMagnitude++;
        
            char dirBackToOrigin = solvedGrid[currentPosition.row, currentPosition.col];
            if (dirBackToOrigin == GridPoints.Origin)
            {
                foundOrigin = currentPosition;
                return;
            }
        
            switch (dirBackToOrigin)
            {
                case GridPoints.DIR_BACK_TO_ORIGIN_RIGHT:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row, currentPosition.col + 1), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationInstruction.NavigationDirection.Left)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationInstruction.NavigationDirection.Left));
                    }
                    else
                    {
                        NavigationInstruction last = originToTarget.Last();
                        last.IncrementMagnitude();
                        originToTarget[^1] = last;
                    }

                    break;
                }

                case GridPoints.DIR_BACK_TO_ORIGIN_DOWN:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row + 1, currentPosition.col), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationInstruction.NavigationDirection.Up)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationInstruction.NavigationDirection.Up));
                    }
                    else
                    {
                        NavigationInstruction last = originToTarget.Last();
                        last.IncrementMagnitude();
                        originToTarget[^1] = last;
                    }
                
                    break;
                }

                case GridPoints.DIR_BACK_TO_ORIGIN_LEFT:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row, currentPosition.col - 1), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationInstruction.NavigationDirection.Right)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationInstruction.NavigationDirection.Right));
                    }
                    else
                    {
                        NavigationInstruction last = originToTarget.Last();
                        last.IncrementMagnitude();
                        originToTarget[^1] = last;
                    }
                
                    break;
                }

                case GridPoints.DIR_BACK_TO_ORIGIN_UP:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row - 1, currentPosition.col), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationInstruction.NavigationDirection.Down)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationInstruction.NavigationDirection.Down));
                    }
                    else
                    {
                        NavigationInstruction last = originToTarget.Last();
                        last.IncrementMagnitude();
                        originToTarget[^1] = last;
                    }
                
                    break;   
                }
            }
        }
    }
}