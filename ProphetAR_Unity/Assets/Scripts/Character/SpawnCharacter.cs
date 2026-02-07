using System.Collections;
using System.Collections.Generic;
using ProphetAR.Coroutines;
using UnityEngine;

namespace ProphetAR
{
    public class SpawnCharacter : MonoBehaviour
    {
        [SerializeField]
        private Level _level = null;

        [SerializeField]
        private Character _characterPrefab = null;

        [SerializeField]
        private Vector2Int _spawnCoordinates = Vector2Int.zero;

        [SerializeField]
        private int _gamePlayerIndex = -1;

        [SerializeField]
        private bool _onInitialize = false;

        private IEnumerator Start()
        {
            if (!_level.IsInitialized)
            {
                yield return new WaitForInitializedLevel();
            }
            
            Spawn();
        }

        public void Spawn()
        {
            Character character = _level.Grid.InstantiateGridObject(_characterPrefab, _spawnCoordinates);
            if (_gamePlayerIndex >= 0)
            {
                _level.Players[_gamePlayerIndex].State.AddCharacter(character);
            }
        }
    }
}