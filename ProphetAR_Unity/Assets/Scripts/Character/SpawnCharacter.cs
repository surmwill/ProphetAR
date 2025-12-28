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
        private bool _onStart = false;

        private void Start()
        {
            if (_onStart)
            {
                Spawn();
            }
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