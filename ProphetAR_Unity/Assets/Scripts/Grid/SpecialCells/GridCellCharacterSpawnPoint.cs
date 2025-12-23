using System;
using UnityEngine;

namespace ProphetAR
{
    public class GridCellCharacterSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private GridCellContent _gridCellContent;
        
        [SerializeField]
        private Vector2Int _coordinates;

        [SerializeField]
        private int _playerIndex = 0;
        
        public Vector2Int Coordinates => _coordinates;

        private Level Level => _gridCellContent.Cell.GridSection.ParentGrid.Level;

        private void Awake()
        {
            Level.AddLevelConfigContributor(this);
        }
        
        private void OnValidate()
        {
            _coordinates = GetComponentInParent<GridCell>().Coordinates;
        }
    }
}