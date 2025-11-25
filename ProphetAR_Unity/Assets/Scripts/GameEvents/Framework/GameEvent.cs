using System;
using System.Collections.Generic;

namespace ProphetAR
{
    /// <summary>
    /// Base game event class used internally to treat all game events (those that pass data and those that don't) under one common reference type.
    /// All game events should either derive from GameEventWithTypedData or GameEventWithoutData
    /// </summary>
    public abstract class GameEvent
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