namespace ProphetAR
{
    public class GameEventCharacterStatsModifiedData
    {
        public Character Character { get; }
        
        public CharacterStats CharacterStats { get; }
        
        public GameEventCharacterStatsModifiedData(Character character, CharacterStats characterStats)
        {
            Character = character;
            CharacterStats = characterStats;
        }
    }
}