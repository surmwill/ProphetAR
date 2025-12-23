using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerState
    {
        public CustomPriorityQueue<MultiGameTurnAction> MultiTurnActions { get; } = new();

        public List<Character> Characters { get; } = new();
        
        public GamePlayer Player { get; }
        
        public GamePlayerState(GamePlayer player, GamePlayerConfig initConfig)
        {
            Player = player;
        }
    }
}