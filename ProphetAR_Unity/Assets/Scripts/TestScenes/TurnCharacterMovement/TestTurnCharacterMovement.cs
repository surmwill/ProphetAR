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
        private List<Character> _player1CharacterPrefabs = null;

        [SerializeField]
        private DebugGamePlayerMaterials _debugGamePlayerMaterials = null;

        private void Start()
        {
            _level.Initialize(new []
            {
                new GamePlayerConfig("Player0", false, _player0CharacterPrefabs),
                new GamePlayerConfig("Player1", false, _player1CharacterPrefabs)
            });

            Character player0Character = _level.Players[0].State.Characters.First();
            Character player1Character = _level.Players[1].State.Characters.First();

            player0Character.GetComponentInChildren<MeshRenderer>().material = _debugGamePlayerMaterials.PlayerMaterials[0];
            player1Character.GetComponentInChildren<MeshRenderer>().material = _debugGamePlayerMaterials.PlayerMaterials[1];
        }
    }
}