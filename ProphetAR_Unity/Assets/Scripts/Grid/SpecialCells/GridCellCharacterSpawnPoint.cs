using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public class GridCellCharacterSpawnPoint : MonoBehaviour, ILevelConfigContributor
    {
        [SerializeField]
        private GridCellContent _gridCellContent = null;

        [SerializeField]
        private CharacterSpawnPoint _spawnPoint = default;

        public string LevelConfigEditedBy => $"Character spawn point. Player Index: {_spawnPoint.PlayerIndex}, Coordinates: {_spawnPoint.Coordinates}";

        private Level Level => _gridCellContent.Cell.GridSection.ParentGrid.Level;

        private void Awake()
        {
            Level.AddLevelConfigContributor(this);
        }
        
        private void OnValidate()
        {
            _spawnPoint.Coordinates = GetComponentInParent<GridCell>().Coordinates;
        }
        
        public void EditLevelConfig(LevelConfig levelConfig)
        {
            if (!levelConfig.PlayerSpawnPoints.TryGetValue(_spawnPoint.PlayerIndex, out List<CharacterSpawnPoint> spawnPoints))
            {
                spawnPoints = new List<CharacterSpawnPoint>();
                levelConfig.PlayerSpawnPoints[_spawnPoint.PlayerIndex] = spawnPoints;
            }
            
            spawnPoints.Add(_spawnPoint);
        }
    }
}