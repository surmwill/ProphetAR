using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerState
    {
        public GamePlayer Player { get; }
        
        public CustomPriorityQueue<GameTurnActionOverTurns> ActionsOverTurns { get; } = new();
        
        public IEnumerable<Character> Characters => _characters;
        
        private readonly List<Character> _characters = new();

        public void AddCharacter(Character character)
        {
            _characters.Add(character);
        }

        public void RemoveCharacter(Character character)
        {
            _characters.Remove(character);
        }
        
        public GamePlayerState(GamePlayer player, GamePlayerConfig initConfig)
        {
            Player = player;
        }
    }
}