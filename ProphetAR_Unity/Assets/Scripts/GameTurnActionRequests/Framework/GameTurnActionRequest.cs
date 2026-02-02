using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public abstract class GameTurnActionRequest : CustomPriorityQueueItem<GameTurnActionRequest>
    {
        private static long _requestNum;
        
        public long RequestNum { get; }
        
        public abstract Type CompletedByGameEventType { get; }
        
        public abstract string Name { get; }
        
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

        /// <summary>
        /// Note that the GameEvent will always match the <see cref="CompletedByGameEventType"/> we provide.
        /// Implement this method for any additional checking.
        /// </summary>
        public virtual bool IsCompletedByGameEvent(GameEvent gameEvent)
        {
            return true;
        }
        
        protected GameTurnActionRequest()
        {
            RequestNum = _requestNum++;
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
        /// The game action can be automatically executed as a coroutine or a (C#) action, one of these needs to be overriden to non-null
        /// </summary>
        public virtual IEnumerator ExecuteAutomaticallyCoroutine { get; } = null;

        /// <summary>
        /// The game action can be automatically executed as a coroutine or a (C#) action, one of these needs to be overriden to non-null
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