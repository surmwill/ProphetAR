using System.Collections;
using GridPathFinding;

namespace ProphetAR
{
    public class CharacterAbilityMovement : CharacterAbility
    {
        public override string Uid => nameof(CharacterAbilityMovement);

        public override int MinNumActionPoints => 1;

        public override IEnumerator AbilityCoroutine => ExecuteAbility();

        private IEnumerator ExecuteAbility()
        {
            (NavigationDestinationSet destinations, GridSlice area) = Character.GetMovementArea();
            Character.Grid.GridPainter.ShowMovementArea(destinations, area);
            yield return null;
        }

        public CharacterAbilityMovement(Character character) : base(character)
        {
        }

    }
}