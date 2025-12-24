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

        [SerializeField]
        private GameObject _debugIndicator = null;
        
        [SerializeField]
        private DebugGamePlayerMaterials _debugGamePlayerMaterials = null;

        public string LevelConfigEditedBy => $"Character spawn point. Player Index: {_spawnPoint.PlayerIndex}, Coordinates: {_spawnPoint.Coordinates}";

        private Level Level => _gridCellContent.Cell.GridSection.ParentGrid.Level;

        private void Awake()
        {
            Level.AddLevelConfigContributor(this);
        }
        
        private void OnValidate()
        {
            if (_gridCellContent == null)
            {
                _gridCellContent = GetComponentInChildren<GridCellContent>();
            }

            if (_gridCellContent != null)
            {
                _spawnPoint.Coordinates = _gridCellContent.Cell.Coordinates;    
            }
            
            _debugIndicator.GetComponentInChildren<MeshRenderer>().sharedMaterial = _debugGamePlayerMaterials.PlayerMaterials[_spawnPoint.PlayerIndex];
        }

        public void ShowDebugIndicator(bool shouldShow)
        {
            _debugIndicator.SetActive(shouldShow);
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