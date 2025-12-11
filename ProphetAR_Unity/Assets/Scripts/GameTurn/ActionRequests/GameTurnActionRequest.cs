using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public abstract class GameTurnActionRequest
    {
        public const int DefaultPriority = int.MaxValue;
        
        public abstract Type CompletedByGameEventType { get; }
        
        public int? Priority { get; }
        
        public Transform FocusTransform { get; }

        public abstract void OnFocusUI();

        public virtual void OnFocusCamera()
        {
            if (FocusTransform == null)
            {
                return;
            }
            
            // Focus camera on FocusTransform
        }

        public abstract void ExecuteAutomatically();

        public abstract Dictionary<string, object> SerializeForServer();

        public virtual bool IsCompletedByGameEvent(GameEvent gameEvent)
        {
            return CompletedByGameEventType == gameEvent.GetType();
        }
    }
}