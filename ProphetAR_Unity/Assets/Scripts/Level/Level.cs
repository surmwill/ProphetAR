using System.Collections.Generic;
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
        
        private Coroutine _nextTurnCoroutine;

        public void Initialize(GamePlayerConfig[] playerConfigs)
        {
            Initialize(new LevelConfig(), playerConfigs);
        }

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

                GamePlayerConfig playerConfig = player.Config;
                for (int i = 0; i < playerCharacterPrefabs.Count; i++)
                {
                    Character characterPrefab = playerCharacterPrefabs[i];
                    CharacterSpawnPoint spawnPoint = spawnPoints[i];
                    CharacterStats characterStats = playerConfig.CharacterStats[i];
                    
                    Character character = Grid.InstantiateGridObject(characterPrefab, spawnPoint.Coordinates);
                    character.Initialize(player, characterStats);
                }
            }
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

        public void StartFirstTurn()
        {
            NextTurn();
        }
        
        public void NextTurn()
        {
            if (_nextTurnCoroutine != null)
            {
                Debug.LogWarning("Turn change is already in progress");
                return;
            }
            
            _nextTurnCoroutine = StartCoroutine(TurnManager.NextTurnCoroutine(() => _nextTurnCoroutine = null));
        }
    }
}