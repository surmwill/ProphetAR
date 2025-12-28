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
        private Vector2Int _targetCoordinates = Vector2Int.one;

        [SerializeField]
        private int _gamePlayerIndex = -1;

        private void Start()
        {
            Character character = _level.Grid.InstantiateGridObject(_characterPrefab, _targetCoordinates);
            if (_gamePlayerIndex >= 0)
            {
                _level.Players[_gamePlayerIndex].State.AddCharacter(character);
            }
        }
    }
}