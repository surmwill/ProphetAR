using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnCharacterMovement : MonoBehaviour
    {
        [SerializeField]
        private Level _level = null;

        [SerializeField]
        private List<Character> _player0CharacterPrefabs = null;

        [SerializeField]
        private List<CharacterStats> _player0CharacterStats = null;
        
        [SerializeField]
        private List<Character> _player1CharacterPrefabs = null;
        
        [SerializeField]
        private List<CharacterStats> _player1CharacterStats = null;

        private IEnumerator Start()
        {
            yield return _level.InitializeCoroutine(new []
            {
                new GamePlayerConfig("Player0", false, _player0CharacterPrefabs, _player0CharacterStats),
                new GamePlayerConfig("Player1", false, _player1CharacterPrefabs, _player1CharacterStats)
            });

            _level.StartFirstTurn();
        }

        private void OnValidate()
        {
            if (_player0CharacterStats.Count < _player0CharacterPrefabs.Count)
            {
                _player0CharacterStats.AddRange(Enumerable.Repeat(new CharacterStats(), _player0CharacterPrefabs.Count - _player0CharacterStats.Count));
            }
            
            if (_player1CharacterStats.Count < _player1CharacterPrefabs.Count)
            {
                _player1CharacterStats.AddRange(Enumerable.Repeat(new CharacterStats(), _player1CharacterPrefabs.Count - _player1CharacterStats.Count));
            }
        }
    }
}