using System;
using System.Collections;
using GridOperations;

namespace ProphetAR
{
    public class CharacterAbilityAttack : CharacterAbility
    {
        public override string Uid => nameof(CharacterAbilityAttack);

        public override string Name => "Attack";

        protected override int DefaultMinNumActionPoints => 2;
        
        protected override AbilityCoroutine AbilityAsCoroutine { get; }

        /*
        private IEnumerator ExecuteAbility(Action onComplete, Action onCancelled)
        {
            (NavigationDestinationSet destinationSet, GridSlice area) = Character.GetMovementArea(Character.Stats.AttackRange);
        }
        */
        
        public CharacterAbilityAttack(Character character) : base(character) { }
    }
}