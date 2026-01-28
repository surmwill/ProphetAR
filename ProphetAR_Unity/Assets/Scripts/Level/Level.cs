using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private CustomGrid _grid = null;

        public static Level Current { get; private set; }

        public CustomGrid Grid => _grid;
        
        public LevelConfig LevelConfig { get; private set; }
        
        public LevelState LevelState { get; private set; }
        
        public GamePlayer[] Players { get; private set; }
        
        public GameEventProcessor EventProcessor { get; } = new();
        
        public GameTurnManager TurnManager { get; private set; }

        public bool IsInitialized => Current != null;

        public bool HasStarted => IsInitialized && TurnManager.CurrTurn != null;
        
        private static readonly List<ILevelLifecycleListener> LevelLifecycleListeners = new();

        private readonly List<ILevelConfigContributor> _levelConfigContributors = new();
        
        private Coroutine _nextTurnCoroutine;

        public IEnumerator InitializeCoroutine(GamePlayerConfig[] playerConfigs)
        {
            yield return InitializeCoroutine(new LevelConfig(), playerConfigs);
        }

        public IEnumerator InitializeCoroutine(LevelConfig levelConfig, GamePlayerConfig[] playerConfigs)
        {
            if (Current != null)
            {
                throw new InvalidOperationException("A level currently exists");
            }
            
            // Initialize the data needed to create the level
            InitializeData(levelConfig, playerConfigs);
            
            // Create the level
            InitializeLevel();
            
            // The game is ready for its first turn
            Current = this;
            foreach (ILevelLifecycleListener lifecycleListener in LevelLifecycleListeners)
            {
                lifecycleListener.OnLevelLifecycleChanged(LevelLifecycleState.Initialized, null, Current);
            }

            // Allow any coroutines waiting on level initialization to complete
            yield return null;
            yield return null;
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
            if (levelConfig.HasBeenEdited)
            {
                Debug.Log(levelConfig.DebugGetEditedBy());   
            }
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
                List<Character> playerCharacterPrefabs = player.Config.CharacterPrefabs;
                if (playerCharacterPrefabs == null || playerCharacterPrefabs.Count == 0)
                {
                    Debug.LogWarning($"No characters found for player {player.Index}: {player.Uid}");
                    continue;
                }
                
                GamePlayerConfig playerConfig = player.Config;
                
                LevelConfig.PlayerSpawnPoints.TryGetValue(player.Index, out List<CharacterSpawnPoint> spawnPoints);
                IEnumerator<Vector2Int> defaultSpawnCoordinates = GetNextDefaultCellSpawnCoordinates();
                
                for (int i = 0; i < playerCharacterPrefabs.Count; i++)
                {
                    Character characterPrefab = playerCharacterPrefabs[i];

                    CharacterSpawnPoint spawnPoint = default;
                    if (i < spawnPoints?.Count)
                    {
                        spawnPoint = spawnPoints[i];
                    }
                    else
                    {
                        defaultSpawnCoordinates.MoveNext();
                        Debug.LogWarning($"Missing spawn point for player {player.Index}: {player.Uid}. Using default {defaultSpawnCoordinates.Current}");
                        spawnPoint = new CharacterSpawnPoint(player.Index, defaultSpawnCoordinates.Current);
                    }
                    
                    
                    Character character = Grid.InstantiateGridObject(characterPrefab, spawnPoint.Coordinates);
                    character.Initialize(player, playerConfig.CharacterStats[i]);
                }
            }

            IEnumerator<Vector2Int> GetNextDefaultCellSpawnCoordinates()
            {
                foreach (GridCell gridCell in Grid)
                {
                    if (gridCell.GridPointProperties.GridPointType == GridPointType.Clear && !gridCell.Content.HasCharacters)
                    {
                        yield return gridCell.Coordinates;
                    }
                }
            }
        }

        public void AddLevelConfigContributor(ILevelConfigContributor levelConfigContributor)
        {
            if (IsInitialized)
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

        private void OnDestroy()
        {
            foreach (ILevelLifecycleListener lifecycleListener in LevelLifecycleListeners)
            {
                lifecycleListener.OnLevelLifecycleChanged(LevelLifecycleState.Destroyed, Current, null);
            }
        }

        public static void RegisterLevelLifecycleListener(ILevelLifecycleListener listener)
        {
            LevelLifecycleListeners.Add(listener);
            
            if (Current != null)
            {
                listener.OnLevelLifecycleChanged(LevelLifecycleState.Initialized, null, Current);
            }
        }

        public static void UnregisterLevelLifecycleListener(ILevelLifecycleListener listener)
        {
            LevelLifecycleListeners.Remove(listener);
        }
    }
}