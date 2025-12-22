using UnityEngine;

namespace ProphetAR
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private CustomGrid _grid = null;

        public CustomGrid Grid => _grid;
        
        public GameEventProcessor EventProcessor { get; } = new();

        public GamePlayer[] Players { get; private set; }
        
        public GameTurnManager TurnManager { get; private set; }

        public void Initialize(LevelConfig levelConfig, GamePlayerConfig[] playerConfigs)
        {
            
            
            Players = new GamePlayer[playerConfigs.Length];
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                Players[i] = new GamePlayer(playerConfigs[i]);
            }

            TurnManager = new GameTurnManager(this, Players);
        }

        public void StartFirstTurn()
        {
            TurnManager.NextTurn();   
        }
    }
}