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
    
        // Non-null. If the origin and the target are the same then this is empty. If a path cannot be made then no instruction set will be returned in the first place
        public List<NavigationInstruction> PathToTarget { get; }
        
        public List<(int row, int col)> PathCoordinates { get; } = new();
        
        public int Magnitude { get; }

        private readonly Dictionary<(int row, int col), int> _coordinateToStepNumber = new();

        public NavigationInstructionSet((int row, int col) origin, (int row, int col) target, List<NavigationInstruction> pathToTarget) :
            this(origin, target, pathToTarget, pathToTarget.Sum(instruction => instruction.Magnitude)) { }

        public NavigationInstructionSet((int row, int col) origin, (int row, int col) target, List<NavigationInstruction> pathToTarget, int magnitude)
        {
            Origin = origin;
            Target = target;
        
            PathToTarget = pathToTarget;
            Magnitude = magnitude;

            (int row, int col) pathCoordinate = origin;
            
            PathCoordinates.Add(pathCoordinate);
            _coordinateToStepNumber.Add(pathCoordinate, 0);

            int stepNumber = 1;
            foreach (NavigationInstruction instruction in pathToTarget)
            {
                for (int i = 0; i < instruction.Magnitude; i++, stepNumber++)
                {
                    pathCoordinate = NavigationDirectionUtils.AddInstructionToTuple(pathCoordinate, instruction.Direction);
                    
                    PathCoordinates.Add(pathCoordinate);
                    _coordinateToStepNumber.Add(pathCoordinate, stepNumber);
                }
            }
        }

        public int GetStepNumberForCoordinate((int row, int col) coordinate)
        {
            if (!_coordinateToStepNumber.TryGetValue(coordinate, out int stepNumber))
            {
                throw new ArgumentException("Coordinate not contained in path");
            }

            return stepNumber;
        }
        
        public bool ContainsCoordinate((int row, int col) coordinate)
        {
            return _coordinateToStepNumber.ContainsKey(coordinate);
        }

        public string ListInstructionsString()
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < PathToTarget.Count; i++)
            {
                NavigationInstruction navigationInstruction = PathToTarget[i];
                sb.AppendLine($"{i}: {navigationInstruction.Direction},{navigationInstruction.Magnitude}");
            }
            
            return sb.ToString();
        }

        public string ListPathCoordinatesString()
        {
            return PathCoordinates.Aggregate(string.Empty, (current, coordinates) 
                => $"({coordinates.row},{coordinates.col})" + (current != string.Empty ? " -> " : string.Empty) + current);
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