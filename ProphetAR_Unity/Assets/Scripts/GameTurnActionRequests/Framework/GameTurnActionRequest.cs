using System;
using System.Collections;
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

        public virtual Dictionary<string, object> ServerSerializedGameStateChanges()
        {
            // To be implemented if we ever need multiplayer or cheat detection
            return null;
        }

        public virtual bool IsCompletedByGameEvent(GameEvent gameEvent)
        {
            return CompletedByGameEventType == gameEvent.GetType();
        }
        
        #region AI_Execution

        public bool DidAutomaticExecutionFail => OnAutomaticExecutionFailedManualAction != null;

        public virtual GameTurnActionRequest OnAutomaticExecutionFailedManualAction { get; } = null;

        public AutomaticExecutionType AutomaticExecutionMethod
        {
            get
            {
                if (ExecuteAutomaticallyCoroutine == null && ExecuteAutomaticallyAction == null)
                {
                    Debug.LogWarning("Action request is trying to execute automatically, but no logic has been given");
                }

                return ExecuteAutomaticallyCoroutine != null ? AutomaticExecutionType.Coroutine : AutomaticExecutionType.Action;
            }
        }

        /// <summary>
        /// The game action can be automatically executed as a coroutine or a (C#) action, whichever one is non-null
        /// </summary>
        public virtual IEnumerator ExecuteAutomaticallyCoroutine { get; } = null;

        /// <summary>
        /// The game action can be automatically executed as a coroutine or a (C#) action, whichever one is non-null
        /// </summary>
        public virtual Action ExecuteAutomaticallyAction { get; } = null;

        public enum AutomaticExecutionType
        {
            Coroutine = 0,
            Action = 1,
        }
        
        #endregion
    }
}