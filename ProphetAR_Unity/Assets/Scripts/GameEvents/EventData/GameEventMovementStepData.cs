using UnityEngine;

namespace ProphetAR
{
    public struct GameEventMovementStepData
    {
        public Vector2 From { get; }
        
        public Vector2 To { get; }

        public GameEventMovementStepData(Vector2 from, Vector2 to)
        {
            From = from;
            To = to;
        }
    }
}