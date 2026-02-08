using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GridOperations.NavigationInstruction;

namespace GridOperations
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

            if (serializedGrid.Origin.HasValue)
            {
                grid[serializedGrid.Origin.Value.row, serializedGrid.Origin.Value.col] = GridPoints.Origin;
            }

            if (serializedGrid.Target.HasValue)
            {
                grid[serializedGrid.Target.Value.row, serializedGrid.Target.Value.col] = GridPoints.Target;
            }

            if (serializedGrid.Obstacles.Count > 0)
            {
                foreach ((int row, int col) in serializedGrid.Obstacles.Where(obstacle => IsPointInGrid(obstacle, gridDimensions)))
                {
                    grid[row, col] = GridPoints.Obstacle;
                }   
            }

            if (serializedGrid.ModificationSteps.Count > 0)
            {
                foreach (ModificationStep modificationStep in serializedGrid.ModificationSteps.Values.Where(modificationStep => IsPointInGrid(modificationStep.Coordinates, gridDimensions)))
                {
                    grid[modificationStep.Coordinates.row, modificationStep.Coordinates.col] = GridPoints.ModificationStepValueToGridPoint(modificationStep.Value);
                }   
            }
        }

        private static char[,] DeserializeGrid(SerializedGrid serializedGrid)
        {
            char[,] grid = CreateOrClearGrid(serializedGrid.Dimensions.numRows, serializedGrid.Dimensions.numCols);
            AddSpecialGridPoints(grid, serializedGrid);
            return grid;
        }

        public static NavigationDestinationSet GetPathsFrom(SerializedGrid serializedGrid, int maxNumSteps, bool excludeOrigin = false)
        {
            if (!serializedGrid.Origin.HasValue)
            {
                throw new ArgumentException("No origin");
            }
            
            (int row, int col) origin = serializedGrid.Origin.Value;
            if (!IsPointInGrid(origin, serializedGrid.Dimensions))
            {
                throw new ArgumentException($"Origin is not in the grid: {origin}");
            }

            maxNumSteps = Math.Max(maxNumSteps, 0);
            char[,] solvedGrid = DeserializeGrid(serializedGrid);
            
            Dictionary<(int row, int col), NavigationDestination> navigationDestinations = new Dictionary<(int row, int col), NavigationDestination>();
        
            NavigationDestination navigationDestination = new NavigationDestination(origin, origin, 0, solvedGrid);
            if (!excludeOrigin)
            {
                navigationDestinations[origin] = navigationDestination;   
            }
        
            Queue<(int row, int col, int numSteps)> nextPositions = new Queue<(int row, int col, int numSteps)>();
            nextPositions.Enqueue((origin.row, origin.col, 0));
        
            while (nextPositions.Count > 0)
            {
                (int row, int col, int numSteps) currentPosition = nextPositions.Dequeue();
            
                if (currentPosition.col > 0 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row, currentPosition.col - 1]))
                {
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row, currentPosition.col - 1], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row, currentPosition.col - 1] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_RIGHT;
                        navigationDestination = new NavigationDestination((currentPosition.row, currentPosition.col - 1), origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[(currentPosition.row, currentPosition.col - 1)] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row, currentPosition.col - 1, currentPosition.numSteps + modifiedNumSteps));  
                    }
                }

                if (currentPosition.row < serializedGrid.Dimensions.numRows - 1 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row + 1, currentPosition.col]))
                { 
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row + 1, currentPosition.col], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row + 1, currentPosition.col] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_UP;
                        navigationDestination = new NavigationDestination((currentPosition.row + 1, currentPosition.col), origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[(currentPosition.row + 1, currentPosition.col)] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row + 1, currentPosition.col, currentPosition.numSteps + modifiedNumSteps));  
                    }
                }

                if (currentPosition.col < serializedGrid.Dimensions.numCols - 1 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row, currentPosition.col + 1]))
                {
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row, currentPosition.col + 1], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row, currentPosition.col + 1] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_LEFT;
                        navigationDestination = new NavigationDestination((currentPosition.row, currentPosition.col + 1), origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[(currentPosition.row, currentPosition.col + 1)] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row, currentPosition.col + 1, currentPosition.numSteps + modifiedNumSteps));
                    }
                }

                if (currentPosition.row > 0 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row - 1, currentPosition.col]))
                {
                    if (!GridPoints.IsModificationStep(solvedGrid[currentPosition.row - 1, currentPosition.col], out int modifiedNumSteps))
                    {
                        modifiedNumSteps = 1;
                    }

                    if (currentPosition.numSteps + modifiedNumSteps <= maxNumSteps)
                    {
                        solvedGrid[currentPosition.row - 1, currentPosition.col] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_DOWN;
                        navigationDestination = new NavigationDestination((currentPosition.row - 1, currentPosition.col), origin, currentPosition.numSteps + modifiedNumSteps, solvedGrid);
                        navigationDestinations[(currentPosition.row - 1, currentPosition.col)] = navigationDestination;
                        nextPositions.Enqueue((currentPosition.row - 1, currentPosition.col, currentPosition.numSteps + modifiedNumSteps));
                    }
                }
            }
            
            return new NavigationDestinationSet(origin, maxNumSteps, navigationDestinations, serializedGrid);
        }

        public static NavigationInstructionSet GetPathTo(SerializedGrid serializedGrid)
        {
            if (!serializedGrid.Origin.HasValue)
            {
                throw new ArgumentException("No origin");
            }

            if (!serializedGrid.Target.HasValue)
            {
                throw new ArgumentException("No target");
            }

            (int row, int col) origin = serializedGrid.Origin.Value;
            (int row, int col) target = serializedGrid.Target.Value;

            if (!IsPointInGrid(origin, serializedGrid.Dimensions))
            {
                throw new ArgumentException($"Origin is not in the grid: {origin}");
            }
        
            if (!IsPointInGrid(target, serializedGrid.Dimensions))
            {
                return null;
            }
        
            if (serializedGrid.Origin == serializedGrid.Target)
            {
                return new NavigationInstructionSet(origin, target, new List<NavigationInstruction>());
            }

            char[,] solvedGrid = DeserializeGrid(serializedGrid);
        
            Queue<(int row, int col)> nextPositions = new Queue<(int row, int col)>();
            nextPositions.Enqueue(origin);
        
            bool found = false;
            while (nextPositions.Count > 0)
            {
                (int row, int col) currentPosition = nextPositions.Dequeue();
                if (currentPosition == serializedGrid.Target)
                {
                    found = true;
                    break;
                }
            
                if (currentPosition.col > 0 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row, currentPosition.col - 1]))
                {
                    solvedGrid[currentPosition.row, currentPosition.col - 1] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_RIGHT;
                    nextPositions.Enqueue((currentPosition.row, currentPosition.col - 1));   
                }

                if (currentPosition.row < serializedGrid.Dimensions.numRows - 1 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row + 1, currentPosition.col]))
                {
                    solvedGrid[currentPosition.row + 1, currentPosition.col] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_UP;
                    nextPositions.Enqueue((currentPosition.row + 1, currentPosition.col));   
                }

                if (currentPosition.col < serializedGrid.Dimensions.numCols - 1 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row, currentPosition.col + 1]))
                {
                    solvedGrid[currentPosition.row, currentPosition.col + 1] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_LEFT;
                    nextPositions.Enqueue((currentPosition.row, currentPosition.col + 1));   
                }

                if (currentPosition.row > 0 && GridPoints.GridSolvingCanVisitPoint(solvedGrid[currentPosition.row - 1, currentPosition.col]))
                {
                    solvedGrid[currentPosition.row - 1, currentPosition.col] = GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_DOWN;
                    nextPositions.Enqueue((currentPosition.row - 1, currentPosition.col));   
                }
            }
            
            return found ? ReverseBuildPathToOrigin(target, solvedGrid) : null;
        }

        public static NavigationInstructionSet ReverseBuildPathToOrigin((int row, int col) fromPosition, char[,] solvedGrid)
        {
            int totalMagnitude = 0;
            List<NavigationInstruction> navigationInstructions = new List<NavigationInstruction>();
            (int row, int col) foundOrigin = default;
            
            ReverseAndRememberDirectionBack(fromPosition, navigationInstructions, ref totalMagnitude, ref foundOrigin, solvedGrid);
            return new NavigationInstructionSet(foundOrigin, fromPosition, navigationInstructions, totalMagnitude);
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
                case GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_RIGHT:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row, currentPosition.col + 1), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationDirection.Left)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationDirection.Left));
                    }
                    else
                    {
                        NavigationInstruction last = originToTarget.Last();
                        last.IncrementMagnitude();
                        originToTarget[^1] = last;
                    }

                    break;
                }

                case GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_DOWN:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row + 1, currentPosition.col), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationDirection.Up)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationDirection.Up));
                    }
                    else
                    {
                        NavigationInstruction last = originToTarget.Last();
                        last.IncrementMagnitude();
                        originToTarget[^1] = last;
                    }
                
                    break;
                }

                case GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_LEFT:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row, currentPosition.col - 1), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationDirection.Right)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationDirection.Right));
                    }
                    else
                    {
                        NavigationInstruction last = originToTarget.Last();
                        last.IncrementMagnitude();
                        originToTarget[^1] = last;
                    }
                
                    break;
                }

                case GridPoints.GRID_SOLVING_DIR_BACK_TO_ORIGIN_UP:
                {
                    ReverseAndRememberDirectionBack((currentPosition.row - 1, currentPosition.col), originToTarget, ref totalMagnitude, ref foundOrigin, solvedGrid);
                    if (originToTarget.Count == 0 || originToTarget.Last().Direction != NavigationDirection.Down)
                    {
                        originToTarget.Add(new NavigationInstruction(NavigationDirection.Down));
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