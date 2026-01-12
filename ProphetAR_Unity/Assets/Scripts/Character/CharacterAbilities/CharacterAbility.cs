using System;
using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public abstract class CharacterAbility
    {
        public abstract string Uid { get; }

        public abstract int MinNumActionPoints { get; }
        
        public bool Cancelled { get; protected set; }

        public Character Character { get; }
        
        protected delegate IEnumerator CharacterAbilityCoroutine(Action onComplete = null, Action onCancelled = null); 
        
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
        /// Note there's only something to cancel if the ability is a Coroutine
        /// </summary>
        public virtual void Cancel()
        {
        }
        
        public void Execute()
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
            }
            else if (AbilityCoroutine != null)
            {
                Character.StartCoroutine(ExecuteInner());
            }
        }

        private IEnumerator ExecuteInner()
        {
            Character.CurrentlyExecutingAbilities.Add(this);
            
            yield return AbilityCoroutine;
            
            Character.CurrentlyExecutingAbilities.Remove(this);
        }

        protected CharacterAbility(Character character)
        {
            Character = character;
        }
    }
}