using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerState
    {
        public CustomPriorityQueue<MultiGameTurnAction> MultiTurnActions { get; } = new();
        
        public List<Character> Characters { get; private set; } 
        
        public GamePlayer Player { get; }
        
        public GamePlayerState(GamePlayer player)
        {
            Player = player;
        }
        
        public void InitializeFromConfiguration(GamePlayerConfig config)
        {
            Characters = config.Characters;
        }
    }
}