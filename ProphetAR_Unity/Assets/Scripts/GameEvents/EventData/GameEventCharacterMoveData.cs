using UnityEngine;

namespace ProphetAR
{
    public class GameEventCharacterMoveData
    {
        public Vector2 From { get; }
        
        public Vector2 To { get; }

        public GameEventCharacterMoveData(Vector2 from, Vector2 to)
        {
            From = from;
            To = to;
        }
    }
}