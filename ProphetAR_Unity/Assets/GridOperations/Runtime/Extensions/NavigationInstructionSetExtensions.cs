using System.Collections.Generic;
using System.Linq;
using GridOperations;
using UnityEngine;

namespace ProphetAR
{
    public static class NavigationInstructionSetExtensions
    {
        /// <summary>
        /// Breaks the instruction set up into segments, each segment ending on the splitOn coordinate. The start of each segment begins on the
        /// end of the previous segment. Therefore, the individual segments overlap like hinges and can be joined together.
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

            // Handle splitting on the origin.
            // Usually a split involves starting from the end of the previous split and continuing on to the end of the current split. This implies a magnitude > 0.
            // If we split on the origin, this a special case with magnitude = 0
            if (currSplitOn == instructionSet.Origin)
            {
                splitInstructionSets.Add(new NavigationInstructionSet(instructionSet.Origin, instructionSet.Origin, new List<NavigationInstruction>()));
                
                splitIndex = GetNextSplitIndex(splitIndex + 1);
                if (splitIndex == -1)
                {
                    return splitInstructionSets;
                }
                
                currSplitOn = splitOnFirstToLast[splitIndex];
                currSplitOrigin = instructionSet.Origin;
            }
            
            foreach (NavigationInstruction instruction in instructionSet.PathToTarget)
            {
                NavigationInstruction.NavigationDirection currDirection = instruction.Direction;
                
                int lastDirectionSplitMagnitude = 0;     
                for (int currMagnitude = 1; currMagnitude <= instruction.Magnitude; currMagnitude++)
                {
                    pathCoordinateIndex++;
                    (int row, int col) pathCoordinates = instructionSet.PathCoordinates[pathCoordinateIndex];
                    
                    // Found split. Create a new instruction set for getting to that split
                    if (pathCoordinates == currSplitOn)
                    {
                        currSplitInstructions.Add(new NavigationInstruction(currDirection, currMagnitude - lastDirectionSplitMagnitude)); 
                        splitInstructionSets.Add(new NavigationInstructionSet(currSplitOrigin, currSplitOn, currSplitInstructions));
                        
                        // Nothing left for splitting
                        splitIndex = GetNextSplitIndex(splitIndex + 1);
                        if (splitIndex == -1)
                        {
                            break;
                        }
                        
                        lastDirectionSplitMagnitude = currMagnitude;
                        
                        currSplitOn = splitOnFirstToLast[splitIndex];
                        currSplitOrigin = pathCoordinates;

                        currSplitInstructions = new List<NavigationInstruction>();
                    }
                }

                // Nothing left for splitting
                if (splitIndex == -1)
                {
                    break;
                }
                
                // Changed of direction
                int remainingSplitMagnitude = instruction.Magnitude - lastDirectionSplitMagnitude;
                if (remainingSplitMagnitude > 0)
                {
                    currSplitInstructions.Add(new NavigationInstruction(currDirection, remainingSplitMagnitude));   
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

        /// <summary>
        /// Breaks the instruction set up into segments which only go in a single direction.
        /// Note the end of the previous segment forms the start of the next. Therefore the segments overlap and can be joined together 
        /// </summary>
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