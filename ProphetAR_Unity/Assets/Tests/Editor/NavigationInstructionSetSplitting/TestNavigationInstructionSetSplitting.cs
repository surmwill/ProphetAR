using System.Collections.Generic;
using GridPathFinding;
using NUnit.Framework;

using static GridPathFinding.NavigationInstruction;

namespace ProphetAR.Tests.NAvigationInstructionSetSplitting
{
    public class TestNavigationInstructionSetSplitting
    {
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
            
            List<NavigationInstructionSet> split = navigationInstructionSet.SplitOnCoordinates(new[] { (1, 0), (2, 2) });
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
            
            Assert.IsTrue(setTwo.Origin == (1, 1));
            Assert.IsTrue(setTwo.Target == (2, 2));
            Assert.IsTrue(setTwo.PathCoordinates.Count == 2);
            
            Assert.IsTrue(setTwo.PathToTarget.Count == 2);
            Assert.IsTrue(setTwo.PathToTarget[0].Direction == NavigationDirection.Right);
            Assert.IsTrue(setTwo.PathToTarget[0].Magnitude == 1);
            
            Assert.IsTrue(setTwo.PathToTarget[1].Direction == NavigationDirection.Down);
            Assert.IsTrue(setTwo.PathToTarget[1].Magnitude == 1);
            
        }
    }
}