using System.Linq;
using Swill.Recycler;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilitiesRecyclerUI : RecyclerScrollRect<string, TestTurnScreenCharacterAbilitiesRecyclerUIData>
    {
        private Character _currCharacter;

        public void ShowAbilitiesForCharacter(Character character)
        {
            if (_currCharacter == character)
            {
                return;
            }
            _currCharacter = character;
            
            Clear();
            AppendEntries(character.Abilities.Select(ability => new TestTurnScreenCharacterAbilitiesRecyclerUIData(ability)));
        }
    }
}