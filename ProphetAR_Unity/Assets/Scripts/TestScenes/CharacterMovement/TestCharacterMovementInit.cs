using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public class TestCharacterMovementInit : MonoBehaviour
    {
        [SerializeField]
        private Level _level = null;

        [SerializeField]
        private List<Character> _characterPrefabs = null;

        [SerializeField]
        private List<Vector2Int> _spawnCoordinates = null;

        private List<TestCharacterMovement> CharactersMovement =>
            _characters.Select(character => character.GetComponent<TestCharacterMovement>()).ToList();
        
        private readonly List<Character> _characters = new();

        private void Start()
        {
            for (int i = 0; i < _characterPrefabs.Count; i++)
            {
                _characters.Add(_level.Grid.InstantiateGridObject(_characterPrefabs[i], _spawnCoordinates[i]).GetComponent<Character>());
            }
        }

        private void Update()
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
            CharactersMovement.ForEach(characterMovement => characterMovement.MoveToCoordinates());
        }
        
        private void WalkCharactersToCoordinates()
        {
            CharactersMovement.ForEach(characterMovement => characterMovement.WalkToCoordinates());
        }
    }
}