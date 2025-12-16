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
    
        // Never null. If the origin and the target are the same then this is empty. If a path cannot be made then no instruction set will be returned in the first place
        public List<NavigationInstruction> PathToTarget { get; }
        
        public List<(int row, int col)> PathCoordinates { get; } = new();
        
        public int Magnitude { get; }

        private readonly HashSet<(int row, int col)> _includesCoordinates;

        public NavigationInstructionSet((int row, int col) origin, (int row, int col) target, List<NavigationInstruction> pathToTarget) :
            this(origin, target, pathToTarget, pathToTarget.Sum(instruction => instruction.Magnitude)) { }

        public NavigationInstructionSet((int row, int col) origin, (int row, int col) target, List<NavigationInstruction> pathToTarget, int magnitude)
        {
            Origin = origin;
            Target = target;
        
            PathToTarget = pathToTarget;
            Magnitude = magnitude;

            (int row, int col) path = origin;
            PathCoordinates.Add(path);
            
            foreach (NavigationInstruction instruction in pathToTarget)
            {
                for (int i = 0; i < instruction.Magnitude; i++)
                {
                    path = NavigationDirectionUtils.AddInstructionToTuple(path, instruction.Direction);
                    PathCoordinates.Add(path);
                }
            }

            _includesCoordinates = new HashSet<(int row, int col)>(PathCoordinates);
        }

        public bool IncludesCoordinates((int row, int col) coordinates)
        {
            return _includesCoordinates.Contains(coordinates);
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