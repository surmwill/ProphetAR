using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public abstract class GameTurnActionRequest : CustomPriorityQueueItem<GameTurnActionRequest>
    {
        public abstract Type CompletedByGameEventType { get; }
        
        public virtual Transform FocusTransform { get; }

        public virtual void OnFocusCamera()
        {
            if (FocusTransform == null)
            {
                return;
            }
            
            // Focus camera on FocusTransform
        }
        
        public abstract void OnFocusUI();

        public abstract void ExecuteAutomatically();

        public abstract Dictionary<string, object> SerializeForServer();

        public virtual bool IsCompletedByGameEvent(GameEvent gameEvent)
        {
            return CompletedByGameEventType == gameEvent.GetType();
        }
    }
}