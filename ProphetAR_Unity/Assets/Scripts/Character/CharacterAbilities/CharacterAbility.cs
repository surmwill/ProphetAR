using System;
using System.Collections;

namespace ProphetAR
{
    public abstract class CharacterAbility : Ability
    {
        public Character Character { get; }
        
        public override void Execute(Action onComplete = null, Action onCancelled = null)
        {
            base.Execute(onComplete, onCancelled);
            
            if (AbilityAsAction != null)
            {
                AbilityAsAction.Invoke();
                onComplete?.Invoke();
            }
            else if (AbilityAsCoroutine != null)
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
                yield return AbilityAsCoroutine(() => isComplete = true, () => isCancelled = true);   
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