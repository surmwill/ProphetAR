using System.Collections.Generic;
using GridOperations;
using NUnit.Framework;
using UnityEngine;

using static GridOperations.NavigationInstruction;

namespace ProphetAR.Tests.NavigationInstructionSetSplitting
{
    public class NavigationInstructionSetSplittingTests
    {
        [Test]
        public void TestIndividualSplitting()
        {
            NavigationInstruction navigationInstructionDown1 = new NavigationInstruction(NavigationDirection.Down, 1);
            NavigationInstruction navigationInstructionRight = new NavigationInstruction(NavigationDirection.Right, 2);
            NavigationInstruction navigationInstructionDown2 = new NavigationInstruction(NavigationDirection.Down, 2);
            
            List<NavigationInstruction> navigationInstructions = new List<NavigationInstruction>(new[]
            {
                navigationInstructionDown1,
                navigationInstructionRight,
                navigationInstructionDown2,
            });

            NavigationInstructionSet navigationInstructionSet = new NavigationInstructionSet((0, 0), (1, 2), navigationInstructions);
            List<NavigationInstructionSet> split = navigationInstructionSet.SplitOnCoordinates(new List<(int, int)>
            {
                (0, 0), (1, 0), (1, 1), (1, 2), (2, 2), (3, 2)
            });
            
            // Each point should have its own split. Each split starts at the ending point from the previous split and moves on to its own ending point
            Assert.IsTrue(split.Count == 6);
            
            // For the first split on the origin there is no previous ending point, and we have a special case with a magnitude 0 path
            Debug.Log($"Checking split on origin (coordinate 0)");
            NavigationInstructionSet firstInstructionSet = split[0];
            
            Assert.IsTrue(firstInstructionSet.Origin == (0, 0));
            Assert.IsTrue(firstInstructionSet.PathCoordinates.Count == 1);
            Assert.IsTrue(firstInstructionSet.PathCoordinates[0] == (0, 0));
            Assert.IsTrue(firstInstructionSet.PathToTarget.Count == 0);
            Assert.IsTrue(firstInstructionSet.Magnitude == 0);

            // For the remaining splits they all start from the previous ending point (therefore having magnitude > 0)
            for (int i = 1; i < split.Count; i++)
            {
                (int row, int col) pathCoordinates = navigationInstructionSet.PathCoordinates[i];
                Debug.Log($"Checking split on coordinate {i}: {pathCoordinates}");

                NavigationInstructionSet instructionSet = split[i];
                NavigationInstructionSet prevInstructionSet = split[i - 1];
                
                Assert.IsTrue(instructionSet.Origin == prevInstructionSet.Target);
                Assert.IsTrue(instructionSet.Target == pathCoordinates);
                Assert.IsTrue(instructionSet.PathToTarget.Count == 1, instructionSet.PathToTarget.Count.ToString());
                Assert.IsTrue(instructionSet.PathCoordinates.Count == 2, instructionSet.PathCoordinates.Count.ToString());
                Assert.IsTrue(instructionSet.PathCoordinates[0] == prevInstructionSet.Target);
                Assert.IsTrue(instructionSet.PathCoordinates[1] == pathCoordinates, instructionSet.PathCoordinates[1].ToString() + " " + pathCoordinates);
                Assert.IsTrue(instructionSet.Magnitude == 1);
            }
        }
        
        [Test]
        public void TestSplitting()
        {
            NavigationInstruction navigationInstructionDown1 = new NavigationInstruction(NavigationDirection.Down, 1);
            NavigationInstruction navigationInstructionRight = new NavigationInstruction(NavigationDirection.Right, 2);
            NavigationInstruction navigationInstructionDown2 = new NavigationInstruction(NavigationDirection.Down, 2);

            List<NavigationInstruction> navigationInstructions = new List<NavigationInstruction>(new[]
            {
                navigationInstructionDown1,
                navigationInstructionRight,
                navigationInstructionDown2
            });

            NavigationInstructionSet navigationInstructionSet = new NavigationInstructionSet((0, 0), (3, 2), navigationInstructions);
            
            List<NavigationInstructionSet> split = navigationInstructionSet.SplitOnCoordinates(new List<(int, int)> { (1, 0), (2, 2) });
            Assert.IsTrue(split.Count == 2);

            // Split at (1, 0): origin -> the end of first step of the path
            NavigationInstructionSet setOne = split[0];
            
            Assert.IsTrue(setOne.Origin == (0, 0));
            Assert.IsTrue(setOne.Target == (1, 0));
            Assert.IsTrue(setOne.PathCoordinates.Count == 2);
            
            Assert.IsTrue(setOne.PathToTarget.Count == 1);
            Assert.IsTrue(setOne.PathToTarget[0].Direction == NavigationDirection.Down);
            Assert.IsTrue(setOne.PathToTarget[0].Magnitude == 1);
            
            // Split at (2, 2): the start of the second step of the path -> one off the end of the path
            NavigationInstructionSet setTwo = split[1];
            
            Assert.IsTrue(setTwo.Origin == (1, 0), setTwo.Origin.ToString());
            Assert.IsTrue(setTwo.Target == (2, 2), setTwo.Target.ToString());
            Assert.IsTrue(setTwo.PathCoordinates.Count == 4, setTwo.PathCoordinates.Count.ToString());
            
            Assert.IsTrue(setTwo.PathToTarget.Count == 2, setTwo.PathToTarget.Count.ToString());
            Assert.IsTrue(setTwo.PathToTarget[0].Direction == NavigationDirection.Right);
            Assert.IsTrue(setTwo.PathToTarget[0].Magnitude == 2);
            
            Assert.IsTrue(setTwo.PathToTarget[1].Direction == NavigationDirection.Down);
            Assert.IsTrue(setTwo.PathToTarget[1].Magnitude == 1);
        }
        
        [Test]
        public void TestSplittingOnDirections()
        {
            NavigationInstruction navigationInstructionDown1 = new NavigationInstruction(NavigationDirection.Down, 1);
            NavigationInstruction navigationInstructionRight = new NavigationInstruction(NavigationDirection.Right, 2);
            NavigationInstruction navigationInstructionDown2 = new NavigationInstruction(NavigationDirection.Down, 2);

            List<NavigationInstruction> navigationInstructions = new List<NavigationInstruction>(new[]
            {
                navigationInstructionDown1,
                navigationInstructionRight,
                navigationInstructionDown2
            });

            NavigationInstructionSet navigationInstructionSet = new NavigationInstructionSet((0, 0), (3, 2), navigationInstructions);

            List<NavigationInstructionSet> split = navigationInstructionSet.SplitOnDirectionChanges();
            Assert.IsTrue(split.Count == 3);
            
            NavigationInstructionSet setOne = split[0];
            Assert.IsTrue(setOne.Origin == (0, 0));
            Assert.IsTrue(setOne.Target == (1, 0));
            
            NavigationInstructionSet setTwo = split[1];
            Assert.IsTrue(setTwo.Origin == (1, 0));
            Assert.IsTrue(setTwo.Target == (1, 2));
            
            NavigationInstructionSet setThree = split[2];
            Assert.IsTrue(setThree.Origin == (1, 2));
            Assert.IsTrue(setThree.Target == (3, 2));
        }
        
        [Test]
        public void TestSplitOnEmptyInstructions()
        {
            NavigationInstructionSet navigationInstructionSet = new NavigationInstructionSet((0, 0), (0, 0), new List<NavigationInstruction>());
            
            List<NavigationInstructionSet> split = navigationInstructionSet.SplitOnCoordinates(new List<(int, int)> { (0, 0) });
            Assert.IsTrue(split.Count == 1);
            Assert.IsTrue(split[0].PathCoordinates.Count == 1);
            Assert.IsTrue(split[0].PathCoordinates[0] == (0, 0));
            Assert.IsTrue(split[0].PathToTarget.Count == 0);

            navigationInstructionSet.SplitOnDirectionChanges();
            Assert.IsTrue(split.Count == 1);
            Assert.IsTrue(split[0].PathCoordinates.Count == 1);
            Assert.IsTrue(split[0].PathCoordinates[0] == (0, 0));
            Assert.IsTrue(split[0].PathToTarget.Count == 0);
        }
        
        [Test]
        public void TestEmptySplitOn()
        {
            NavigationInstruction navigationInstructionDown1 = new NavigationInstruction(NavigationDirection.Down, 1);
            NavigationInstruction navigationInstructionRight = new NavigationInstruction(NavigationDirection.Right, 2);
            NavigationInstruction navigationInstructionDown2 = new NavigationInstruction(NavigationDirection.Down, 2);

            List<NavigationInstruction> navigationInstructions = new List<NavigationInstruction>(new[]
            {
                navigationInstructionDown1,
                navigationInstructionRight,
                navigationInstructionDown2
            });

            NavigationInstructionSet navigationInstructionSet = new NavigationInstructionSet((0, 0), (3, 2), navigationInstructions);
            List<NavigationInstructionSet> split = navigationInstructionSet.SplitOnCoordinates(new List<(int row, int col)>());
            
            Assert.IsTrue(split.Count == 1);
            Assert.IsTrue(split[0] == navigationInstructionSet);
        }
    }
}