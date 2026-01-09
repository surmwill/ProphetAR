using Swill.Recycler;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilitiesRecyclerUIData : IRecyclerScrollRectData<string>
    {
        public string Key { get; }
        
        public CharacterAbility CharacterAbility { get; }
        
        public TestTurnScreenCharacterAbilitiesRecyclerUIData(CharacterAbility characterAbility)
        {
            CharacterAbility = characterAbility;
            Key = characterAbility.Uid;
        }
    }
}