using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerState
    {
        public CustomPriorityQueue<MultiGameTurnAction> MultiTurnActions { get; } = new();
        
        public List<Character> Characters { get; } 
        
        public GamePlayer Player { get; }
        
        public GamePlayerState(GamePlayer player, GamePlayerConfig initFromConfig)
        {
            Player = player;
            
            // Initialize fields from the configuration
            Characters = initFromConfig.Characters;
        }
    }
}