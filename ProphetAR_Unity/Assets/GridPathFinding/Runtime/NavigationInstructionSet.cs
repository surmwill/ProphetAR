using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridPathFinding
{
    public class NavigationInstructionSet : IEnumerable<NavigationInstruction>
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

        public string ShowInstructions()
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < PathToTarget.Count; i++)
            {
                NavigationInstruction navigationInstruction = PathToTarget[i];
                sb.AppendLine($"{i}: {navigationInstruction.Direction},{navigationInstruction.Magnitude}");
            }
            
            return sb.ToString();
        }

        public IEnumerator<NavigationInstruction> GetEnumerator()
        {
            return PathToTarget.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}