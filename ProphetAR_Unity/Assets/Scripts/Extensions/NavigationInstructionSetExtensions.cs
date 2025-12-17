using System;
using System.Collections.Generic;
using System.Linq;
using GridPathFinding;

namespace ProphetAR
{
    public static class NavigationInstructionSetExtensions
    {
        /// <summary>
        /// Breaks the instruction set up into segments, each ending on (and including) the splitOn coordinates.
        /// </summary>
        public static List<NavigationInstructionSet> SplitOnCoordinates(this NavigationInstructionSet instructionSet, IEnumerable<(int row, int col)> splitOn)
        {
            List<(int row, int col)> splitOnFirstToLast = splitOn.OrderBy(instructionSet.GetStepNumberForCoordinate).Distinct().ToList();

            // Nothing to split
            int splitIndex = GetNextSplitIndex(0);
            if (splitIndex == -1)
            {
                return new List<NavigationInstructionSet>(new[] { instructionSet });
            }

            List<NavigationInstructionSet> splitInstructionSets = new List<NavigationInstructionSet>();
            List<NavigationInstruction> currSplitInstructions = new List<NavigationInstruction>();
            
            int pathCoordinateIndex = 0;
            (int row, int col) currSplitOrigin = instructionSet.Origin;
            (int row, int col) currSplitOn = splitOnFirstToLast[splitIndex];
            
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
                        
                        // Nothing left for splitting
                        splitIndex = GetNextSplitIndex(splitIndex + 1);
                        if (splitIndex == -1)
                        {
                            break;
                        }
                        
                        lastSplitMagnitude = currMagnitude;
                        
                        currSplitOn = splitOnFirstToLast[splitIndex];
                        currSplitOrigin = instructionSet.PathCoordinates[pathCoordinateIndex];
                        
                        currSplitInstructions.Clear();
                    }
                }

                // Nothing left for splitting
                if (splitIndex == -1)
                {
                    break;
                }
                
                // Change of direction
                if (currSplitInstructions.Count > 0)
                {
                    currSplitInstructions.Add(new NavigationInstruction(currDirection, instruction.Magnitude - lastSplitMagnitude));
                }
            }

            int GetNextSplitIndex(int startAtIndex)
            {
                for (int i = startAtIndex; i < splitOnFirstToLast.Count; i++)
                {
                    if (instructionSet.ContainsCoordinate(splitOnFirstToLast[i]))
                    {
                        return i;
                    }
                }

                return -1;
            }

            return splitInstructionSets;
        }

        public static List<NavigationInstructionSet> SplitOnDirectionChanges(this NavigationInstructionSet instructionSet, IEnumerable<(int row, int col)> alsoSplitOn = null)
        {
            List<(int row, int col)> lastCoordsBeforeDirectionChange = new List<(int row, int col)>();

            int totalSteps = 0;
            foreach (NavigationInstruction navigationInstruction in instructionSet.PathToTarget)
            {
                totalSteps += navigationInstruction.Magnitude;
                lastCoordsBeforeDirectionChange.Add(instructionSet.PathCoordinates[totalSteps]);
            }

            return SplitOnCoordinates(instructionSet, lastCoordsBeforeDirectionChange.Concat(alsoSplitOn ?? Enumerable.Empty<(int row, int col)>()));
        }
    }
}