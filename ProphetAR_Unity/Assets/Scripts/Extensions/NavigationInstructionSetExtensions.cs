using System;
using System.Collections.Generic;
using GridPathFinding;

namespace ProphetAR
{
    public static class NavigationInstructionSetExtensions
    {
        /// <summary>
        /// Breaks the instruction set up into segments, each ending on (and including) the splitOn coordinates.
        /// The splitOn coordinates must be ordered by their position along the path (first to last).
        /// </summary>
        public static List<NavigationInstructionSet> SplitOnCoordinates(this NavigationInstructionSet instructionSet, List<(int, int)> splitOnFirstToLast)
        {
            (int row, int col)? lastSplitOn = null;
            int? numStepsLastSplit = null;
            
            foreach ((int row, int col) splitOn in splitOnFirstToLast)
            {
                if (lastSplitOn == splitOn)
                {
                    throw new ArgumentException($"Expected distinct coordinates to split on. Duplicate: {splitOn}");
                }
                
                if (!instructionSet.ContainsCoordinate(splitOn))
                {
                    throw new ArgumentException($"The instruction set does not contain the coordinate {splitOn} to split on: {instructionSet.ListPathCoordinatesString()}");
                }

                int numStepsCurrSplit = instructionSet.GetStepNumberForCoordinate(splitOn);
                if (numStepsLastSplit >= numStepsCurrSplit)
                {
                    throw new ArgumentException($"The coordinates to split on must be ordered by their position along the path: {instructionSet.ListPathCoordinatesString()}");
                }

                lastSplitOn = splitOn;
                
            }

            // Splitting on the origin which has no paths (technically ok but useless)
            if (instructionSet.PathToTarget.Count == 0)
            {
                return new List<NavigationInstructionSet>(new[] { instructionSet });
            }

            List<NavigationInstructionSet> splitInstructionSets = new List<NavigationInstructionSet>();

            int pathCoordinateIndex = 0;
            int splitIndex = 0;
            
            (int row, int col) currSplitOrigin = instructionSet.Origin;
            (int row, int col) currSplitOn = splitOnFirstToLast[splitIndex];
            
            List<NavigationInstruction> currSplitInstructions = new List<NavigationInstruction>();
            
            foreach (NavigationInstruction instruction in instructionSet.PathToTarget)
            {
                NavigationInstruction.NavigationDirection currDirection = instruction.Direction;
                int lastSplitMagnitude = 0;
                
                for (int currMagnitude = 1; currMagnitude <= instruction.Magnitude; currMagnitude++)
                {
                    pathCoordinateIndex++;
                    
                    // Found split. Create a new instruction set for getting to that split
                    if (instructionSet.PathCoordinates[pathCoordinateIndex] == currSplitOn)
                    {
                        currSplitInstructions.Add(new NavigationInstruction(currDirection, currMagnitude - lastSplitMagnitude));
                        splitInstructionSets.Add(new NavigationInstructionSet(currSplitOrigin, currSplitOn, currSplitInstructions));
                        
                        // As we verified all split indices are along in the path and ordered by position, so long as there are some left, we know we can loop more 
                        if (splitIndex + 1 == splitOnFirstToLast.Count)
                        {
                            break;
                        }
                        
                        splitIndex++;
                        lastSplitMagnitude = currMagnitude;
                        
                        currSplitOn = splitOnFirstToLast[splitIndex];
                        currSplitOrigin = instructionSet.PathCoordinates[pathCoordinateIndex];
                        
                        currSplitInstructions.Clear();
                    }
                }
                
                // Change of direction
                if (currSplitInstructions.Count > 0)
                {
                    currSplitInstructions.Add(new NavigationInstruction(currDirection, instruction.Magnitude - lastSplitMagnitude));
                }
            }

            return splitInstructionSets;
        }

        public static List<NavigationInstructionSet> SplitOnDirectionChanges(this NavigationInstructionSet instructionSet)
        {
            List<(int row, int col)> lastCoordsBeforeDirectionChange = new List<(int row, int col)>();

            int totalSteps = 0;
            foreach (NavigationInstruction navigationInstruction in instructionSet.PathToTarget)
            {
                totalSteps += navigationInstruction.Magnitude;
                lastCoordsBeforeDirectionChange.Add(instructionSet.PathCoordinates[totalSteps]);
            }

            return SplitOnCoordinates(instructionSet, lastCoordsBeforeDirectionChange);
        }
    }
}