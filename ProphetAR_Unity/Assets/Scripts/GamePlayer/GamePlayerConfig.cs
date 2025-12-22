using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerConfig
    {
        public string PlayerUid { get; }
        
        public bool IsAI { get; }

        public List<Character> Characters { get; } = new();
        
        public GamePlayerConfig(string playerUid, bool isAI, List<Character> characters)
        {
            PlayerUid = playerUid;
            IsAI = isAI;
            Characters = characters;
        }
    }
}