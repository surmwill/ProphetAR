using System.Collections.Generic;
using System.Text;
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

        private readonly List<IContributesToLevelConfig> _levelConfigContributors = new();

        private bool _isInitialized;

        public void Initialize(LevelConfig levelConfig, GamePlayerConfig[] playerConfigs)
        {
            Players = new GamePlayer[playerConfigs.Length];
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                Players[i] = new GamePlayer(playerConfigs[i]);
            }

            TurnManager = new GameTurnManager(this, Players);
            
            // Initialize the level state from the level configuration
            #if UNITY_EDITOR
            StringBuilder levelConfigContributorsString = new StringBuilder("Level config edited by:\n\n");
            #endif
            
            for (int i = 0; i < _levelConfigContributors.Count; i++)
            {
                IContributesToLevelConfig levelConfigContributor = _levelConfigContributors[i];
                
                #if UNITY_EDITOR
                levelConfigContributorsString.AppendLine($"{i+1}: {levelConfigContributor.LevelConfigEditedBy}");
                #endif
                
                // First ask if anything in the level has anything to add to the configuration
                levelConfigContributor.EditLevelConfig(levelConfig);
            }
            
            #if UNITY_EDITOR
            Debug.Log(levelConfigContributorsString);
            #endif
            
            // Set level state
            

            _isInitialized = true;
        }

        public void StartFirstTurn()
        {
            TurnManager.NextTurn();   
        }

        public void AddLevelConfigContributor(IContributesToLevelConfig levelConfigContributor)
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