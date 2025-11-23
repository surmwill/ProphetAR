using System;
using System.Collections.Generic;
using System.Linq;

namespace GridPathFinding
{
    public class NavigationInstructionSet
    {
        public (int row, int col) Origin { get; }
    
        public (int row, int col) Target { get; }
    
        public List<NavigationInstruction> PathToTarget { get; }
        
        public int Magnitude { get; }

        public NavigationInstructionSet((int row, int col) origin, (int row, int col) target, List<NavigationInstruction> pathToTarget) :
            this(origin, target, pathToTarget, pathToTarget.Sum(instruction => instruction.Magnitude)) { }

        public NavigationInstructionSet((int row, int col) origin, (int row, int col) target, List<NavigationInstruction> pathToTarget, int magnitude)
        {
            Origin = origin;
            Target = target;
        
            PathToTarget = pathToTarget;
            Magnitude = magnitude;
        }

        public void PrintInstructions()
        {
            for (int i = 0; i < PathToTarget.Count; i++)
            {
                NavigationInstruction navigationInstruction = PathToTarget[i];
                Console.WriteLine($"{i}: {navigationInstruction.Direction},{navigationInstruction.Magnitude}");
            }
            
            Console.WriteLine();
        }
    }
}