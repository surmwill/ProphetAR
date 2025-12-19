using System;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public class TestCharacterMovementInit : MonoBehaviour
    {
        [SerializeField]
        private Level _level = null;

        [SerializeField]
        private Character[] _characters = null;

        private TestCharacterMovement[] CharacterMovement =>
            _characters.Select(character => character.GetComponent<TestCharacterMovement>()).ToArray();

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                MoveCharactersToCoordinates();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                WalkCharactersToCoordinates();
            }
        }

        private void MoveCharactersToCoordinates()
        {
            Array.ForEach(CharacterMovement, characterMovement => characterMovement.MoveToCoordinates());
        }
        
        private void WalkCharactersToCoordinates()
        {
            Array.ForEach(CharacterMovement, characterMovement => characterMovement.WalkToCoordinates());
        }
    }
}