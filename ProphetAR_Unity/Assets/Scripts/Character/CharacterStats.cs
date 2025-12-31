using System;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public struct CharacterStats
    {
        [SerializeField]
        private int _actionPoints;
        
        [SerializeField]
        private int _maxActionPoints;
        
        [SerializeField]
        private int _actionPointsRegenPerTurn;

        public int ActionPoints
        {
            get => _actionPoints;
            set => _actionPoints = value;
        }

        public int MaxActionPoints
        {
            get => _maxActionPoints; 
            set => _maxActionPoints = value;
        }

        public int ActionPointsRegenPerTurn
        {
            get => _actionPointsRegenPerTurn; 
            set => _actionPointsRegenPerTurn = value;
        }

        public static CharacterStats Sample()
        {
            return new CharacterStats
            {
                ActionPoints = 10,
                MaxActionPoints = 10,
                ActionPointsRegenPerTurn = 10
            };
        }
    }
}