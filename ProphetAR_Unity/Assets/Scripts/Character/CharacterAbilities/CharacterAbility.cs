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
        
        public virtual IEnumerator AbilityCoroutine { get; } = null;

        public virtual Action AbilityAction { get; } = null;

        /// <summary>
        /// The ability can either be a simple callback or a Coroutine performed over time.
        /// One of these needs to be overriden to non-null.
        /// </summary>
        public AbilityExecutionType AbilityExecutionMethod
        {
            get
            {
                if (AbilityCoroutine == null && AbilityAction == null)
                {
                    Debug.LogWarning("Missing ability execution method");
                }

                return AbilityCoroutine != null ? AbilityExecutionType.Coroutine : AbilityExecutionType.Action;
            }
        }

        public virtual void Cancel()
        {
            // There's only something to cancel if the ability is a Coroutine
        }

        protected CharacterAbility(Character character)
        {
            Character = character;
        }

        public enum AbilityExecutionType
        {
            Coroutine = 0,
            Action = 1,
        }
    }
}