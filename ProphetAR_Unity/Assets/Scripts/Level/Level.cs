using UnityEngine;

namespace ProphetAR
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private CustomGrid _grid = null;
        
        public GameEventProcessor EventProcessor { get; } = new();
        
        public LevelState LevelState { get; } = new();

        public LevelConfig LevelConfig { get; private set; } = new();

        public GamePlayer[] Players { get; private set; }
        
        public GameTurnManager TurnManager { get; private set; }

        private LevelEventListener _levelEventListener;

        public void Initialize(LevelConfig levelConfig, GamePlayerConfig[] playerConfigs)
        {
            _levelEventListener = new LevelEventListener(this);
            
            LevelConfig = levelConfig;
            levelConfig.InitializeLevelState(LevelState);
            
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