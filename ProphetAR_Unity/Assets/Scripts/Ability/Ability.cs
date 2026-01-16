using System;
using System.Collections;

namespace ProphetAR
{
    public abstract class Ability
    {
        public abstract string Uid { get; }

        public abstract int MinNumActionPoints { get; }
        
        protected delegate IEnumerator CharacterAbilityCoroutine(Action onComplete, Action onCancelled); 
    }
}