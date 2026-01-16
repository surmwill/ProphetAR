using System;
using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public abstract class CharacterAbility
    {
        public abstract string Uid { get; }

        public abstract int MinNumActionPoints { get; }

        public Character Character { get; }
        
        protected delegate IEnumerator CharacterAbilityCoroutine(Action onComplete, Action onCancelled); 
        
        /// <summary>
        /// The ability can either be a simple callback or a Coroutine performed over time.
        /// One of these needs to be overriden to non-null.
        /// </summary>
        protected virtual CharacterAbilityCoroutine AbilityCoroutine { get; } = null;

        /// <summary>
        /// The ability can either be a simple callback or a Coroutine performed over time.
        /// One of these needs to be overriden to non-null.
        /// </summary>
        protected virtual Action AbilityAction { get; } = null;
        
        /// <summary>
        /// There's only something to cancel if the ability is a Coroutine.
        /// It may also be the case that we're too far into the operation to cancel it, hence why we can return false
        /// </summary>
        public virtual bool TryCancel()
        {
            return false;
        }
        
        public void Execute(Action onComplete = null, Action onCancelled = null)
        {
            if (AbilityCoroutine != null && AbilityAction != null)
            {
                throw new InvalidOperationException("An ability should either be execute by coroutine or action, not both (both are non-null)");
            }
            
            if (AbilityCoroutine == null && AbilityAction == null)
            {
                Debug.LogWarning("Missing ability execution method");
            }

            if (AbilityAction != null)
            {
                AbilityAction.Invoke();
                onComplete?.Invoke();
            }
            else if (AbilityCoroutine != null)
            {
                Character.StartCoroutine(ExecuteInner(onComplete, onCancelled));
            }
        }

        private IEnumerator ExecuteInner(Action onComplete, Action onCancelled)
        {
            bool isComplete = false;
            bool isCancelled = false;
            
            using (new CharacterAbilityExecutionTracker(this))
            {
                yield return AbilityCoroutine(() => isComplete = true, () => isCancelled = true);   
            }
            
            if (isComplete)
            {
                onComplete?.Invoke();
            }
            else if (isCancelled)
            {
                onCancelled?.Invoke();
            }
            // If a complete/cancelled callback has been forgotten, assume it has been completed
            else
            {
                onComplete?.Invoke();
            }
        }

        protected CharacterAbility(Character character)
        {
            Character = character;
        }
        
        public class CharacterAbilityExecutionTracker : IDisposable
        {
            public CharacterAbility Ability { get; }
        
            public CharacterAbilityExecutionTracker(CharacterAbility ability)
            {
                Ability = ability;
                ability.Character.CurrentlyExecutingAbilities.Add(ability);
            }
        
            public void Dispose()
            {
                Ability.Character.CurrentlyExecutingAbilities.Remove(Ability);
            }
        }
    }
}