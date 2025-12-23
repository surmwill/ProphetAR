using System;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public struct CharacterSpawnPoint
    {
        [SerializeField]
        private int _playerIndex;

        [SerializeField]
        private Vector2Int _coordinates;

        public int PlayerIndex => _playerIndex;

        public Vector2Int Coordinates
        {
            get => _coordinates;
            set => _coordinates = value;
        }
    }
}