using System;

namespace ProphetAR
{
    public class CharacterAbilityCompleteTurn : CharacterAbility
    {
        public override string Uid => nameof(CharacterAbilityCompleteTurn);

        public override string Name => "Complete Turn";
        
        protected override int DefaultMinNumActionPoints => 0;

        protected override Action AbilityAsAction => Character.CompleteTurn;

        public CharacterAbilityCompleteTurn(Character character) : base(character) { }
    }
}