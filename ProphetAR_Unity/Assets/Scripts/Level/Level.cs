using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProphetAR
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private CustomGrid _grid = null;

        public CustomGrid Grid => _grid;
        
        public LevelConfig LevelConfig { get; private set; }
        
        public LevelState LevelState { get; private set; }
        
        public GamePlayer[] Players { get; private set; }
        
        public GameEventProcessor EventProcessor { get; } = new();
        
        public GameTurnManager TurnManager { get; private set; }

        private readonly List<ILevelConfigContributor> _levelConfigContributors = new();

        private bool _isInitialized;

        public void Initialize(LevelConfig levelConfig, GamePlayerConfig[] playerConfigs)
        {
            // Initialize the data needed to create the level
            InitializeData(levelConfig, playerConfigs);
            
            // Create the level
            InitializeLevel();
            
            // The game is ready for its first turn
            _isInitialized = true;
        }

        private void InitializeData(LevelConfig levelConfig, GamePlayerConfig[] playerConfigs)
        {
            // Initialize the players and their state given the configurations
            Players = new GamePlayer[playerConfigs.Length];
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                Players[i] = new GamePlayer(i, playerConfigs[i]);
            }
            
            // Initialize the level and its state given the configuration
            for (int i = 0; i < _levelConfigContributors.Count; i++)
            {
                // Different things in the level might add to the configuration (ex: spawn points, special cells)
                levelConfig.Modify(_levelConfigContributors[i]);
            }
            
            #if UNITY_EDITOR
            Debug.Log(levelConfig.DebugGetEditedBy());
            #endif

            LevelConfig = levelConfig;
            LevelState = new LevelState(this, levelConfig);
            
            // Initialize the turn manager
            TurnManager = new GameTurnManager(this, Players);
        }

        private void InitializeLevel()
        {
            foreach (GamePlayer player in Players)
            {
                // Spawn the characters of each player
                if (!LevelConfig.PlayerSpawnPoints.TryGetValue(player.Index, out List<CharacterSpawnPoint> spawnPoints) || spawnPoints.Count == 0)
                {
                    Debug.LogWarning($"No character spawn points for player {player.Index}: {player.Uid}");
                    continue;
                }

                List<Character> playerCharacterPrefabs = player.Config.CharacterPrefabs;
                if (playerCharacterPrefabs == null || playerCharacterPrefabs.Count == 0)
                {
                    Debug.LogWarning($"No characters found for player {player.Index}: {player.Uid}");
                    continue;
                }

                foreach ((Character characterPrefab, CharacterSpawnPoint spawnPoint) in playerCharacterPrefabs.Zip(spawnPoints))
                {
                    player.State.Characters.Add(Grid.InstantiateGridObject(characterPrefab, spawnPoint.Coordinates));
                }
            }
        }

        public void StartFirstTurn()
        {
            TurnManager.NextTurn();   
        }

        public void AddLevelConfigContributor(ILevelConfigContributor levelConfigContributor)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("The level config has already been created and used");
                return;
            }
            
            _levelConfigContributors.Add(levelConfigContributor);
        }
    }
}