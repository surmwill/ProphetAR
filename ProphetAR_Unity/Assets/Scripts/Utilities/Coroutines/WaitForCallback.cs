using System;
using UnityEngine;

namespace ProphetAR.Coroutines
{
    public class WaitForCallback : CustomYieldInstruction
    {
        public bool IsComplete { get; private set; }
        
        public bool IsCancelled { get; private set; }

        public override bool keepWaiting => !IsComplete && !IsCancelled;

        public delegate void ActionWithCompletion(Action onComplete);
        
        public delegate void ActionWithCompletionAndCancel(Action onComplete, Action onCancelled);
        
        public delegate void ActionWithCompletionAndCancelCombined(Action<bool> onComplete);

        public WaitForCallback(ActionWithCompletion actionWithCompletion)
        {
            if (actionWithCompletion == null)
            {
                IsComplete = true;
                return;
            }
            
            actionWithCompletion.Invoke(() => IsComplete = true);
        }
        
        public WaitForCallback(ActionWithCompletionAndCancel actionWithCompletionAndCancel)
        {
            if (actionWithCompletionAndCancel == null)
            {
                IsComplete = true;
                return;
            }
            
            actionWithCompletionAndCancel.Invoke(() => IsComplete = true, () => IsCancelled = true);
        }
        
        public WaitForCallback(ActionWithCompletionAndCancelCombined actionWithCompletionAndCancelCombined)
        {
            if (actionWithCompletionAndCancelCombined == null)
            {
                IsComplete = true;
                return;
            }
            
            actionWithCompletionAndCancelCombined.Invoke(success =>
            {
                IsComplete = success;
                IsCancelled = !success;
            });
        }
    }
}