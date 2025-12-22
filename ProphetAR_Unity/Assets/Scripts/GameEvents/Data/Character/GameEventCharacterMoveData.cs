using System.Collections.Generic;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    public class GameEventCharacterMoveData
    {
        public List<Vector2Int> IntermediateStops { get; } = new();
        
        public Character Character { get; }
        
        public NavigationInstructionSet NavigationInstructions { get; }

        public GameEventCharacterMoveData(Character character, NavigationInstructionSet navigationInstructions)
        {
            Character = character;
            NavigationInstructions = navigationInstructions;
        }
    }
}