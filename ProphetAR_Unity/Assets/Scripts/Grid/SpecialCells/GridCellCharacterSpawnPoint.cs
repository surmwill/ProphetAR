using System;
using UnityEngine;

namespace ProphetAR
{
    public class GridCellCharacterSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _coordinates;

        public Vector2Int Coordinates => _coordinates;
        
        private void OnValidate()
        {
            _coordinates = GetComponentInParent<GridCell>().Coordinates;
        }
    }
}