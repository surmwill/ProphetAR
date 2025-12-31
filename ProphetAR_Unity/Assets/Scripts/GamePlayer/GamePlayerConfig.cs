using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerConfig
    {
        public string PlayerUid { get; }
        
        public bool IsAI { get; }

        public List<Character> CharacterPrefabs { get; }
        
        public List<CharacterStats> CharacterStats { get; } 
        
        public GamePlayerConfig(string playerUid, bool isAI, List<Character> characterPrefabs, List<CharacterStats> characterStats)
        {
            PlayerUid = playerUid;
            IsAI = isAI;
            CharacterPrefabs = characterPrefabs;
            CharacterStats = characterStats;
        }
    }
}