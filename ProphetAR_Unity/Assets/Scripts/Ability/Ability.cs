using System;
using System.Collections;

namespace ProphetAR
{
    public abstract class Ability
    {
        public abstract string Uid { get; }

        public abstract int MinNumActionPoints { get; }
        
        protected delegate IEnumerator AbilityCoroutine(Action onComplete, Action onCancelled); 
        
        // The ability can either be a simple callback or a Coroutine performed over time. One of these needs to be overriden to non-null.
        protected virtual AbilityCoroutine AbilityAsCoroutine { get; } = null;

        // The ability can either be a simple callback or a Coroutine performed over time. One of these needs to be overriden to non-null.
        protected virtual Action AbilityAsAction { get; } = null;
        
        /// <summary>
        /// There's only something to cancel if the ability is a Coroutine.
        /// It may be the case that we're too far into the operation to cancel it, hence why we can return false
        /// </summary>
        public virtual bool TryCancel()
        {
            return false;
        }

        public virtual void Execute(Action onComplete = null, Action onCancelled = null)
        {
            VerifyExecution();
        }

        private void VerifyExecution()
        {
            if (AbilityAsCoroutine != null && AbilityAsAction != null)
            {
                throw new InvalidOperationException("An ability should either be execute by coroutine or action, not both (both are non-null)");
            }
            
            if (AbilityAsCoroutine == null && AbilityAsAction == null)
            {
                throw new InvalidOperationException("Missing ability execution method");
            }
        }
    }
}