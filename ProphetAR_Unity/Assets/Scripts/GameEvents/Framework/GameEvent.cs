using System;
using System.Collections.Generic;

namespace ProphetAR
{
    public class GameEvent
    {
        public GameEventType GameEventType { get; }
        
        public int? CustomPriority { get; }
        
        public void ChangePriority(int? newPriority, Func<IEnumerable<GameEvent>, int> getInsertionIndexAmongSamePriorityEvents = null)
        {
            
        }

        public GameEvent(GameEventType gameEventType, int? customPriority = null)
        {
            GameEventType = gameEventType;
            CustomPriority = customPriority;
            ChangePriority(customPriority);
        }
    }
}