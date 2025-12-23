using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerConfig
    {
        public string PlayerUid { get; }
        
        public bool IsAI { get; }

        public List<Character> CharacterPrefabs { get; }
        
        public GamePlayerConfig(string playerUid, bool isAI, List<Character> characterPrefabs)
        {
            PlayerUid = playerUid;
            IsAI = isAI;
            CharacterPrefabs = characterPrefabs;
        }
    }
}