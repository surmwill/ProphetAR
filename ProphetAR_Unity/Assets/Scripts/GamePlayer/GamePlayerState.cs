using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerState
    {
        public GamePlayer Player { get; }
        
        public CustomPriorityQueue<GameTurnActionOverTurns> ActionsOverTurns { get; } = new();
        
        public IReadOnlyList<Character> Characters => _charactersList;
        
        private readonly List<Character> _charactersList = new();   // List to know order
        private readonly Dictionary<string, Character> _charactersDict = new(); // Dictionary for lookup

        public void AddCharacter(Character character)
        {
            _charactersList.Add(character);
            _charactersDict.Add(character.Uid, character);
            
            character.Player = Player;
        }

        public void RemoveCharacter(Character character)
        {
            _charactersList.Remove(character);
            _charactersDict.Remove(character.Uid);
            
            character.Player = null;
        }

        public bool TryGetCharacter(string characterUid, out Character character)
        {
            return _charactersDict.TryGetValue(characterUid, out character);
        }
        
        public GamePlayerState(GamePlayer player, GamePlayerConfig initConfig)
        {
            Player = player;
        }
    }
}