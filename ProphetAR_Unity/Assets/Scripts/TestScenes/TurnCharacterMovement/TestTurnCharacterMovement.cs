using System.Collections.Generic;
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
        private List<Character> _player1CharacterPrefabs = null;

        private void Start()
        {
            _level.Initialize(new []
            {
                new GamePlayerConfig("Player0", false, _player0CharacterPrefabs),
                new GamePlayerConfig("Player1", false, _player1CharacterPrefabs)
            });
        }
    }
}