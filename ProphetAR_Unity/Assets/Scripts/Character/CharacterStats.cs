using System;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public class CharacterStats
    {
        [SerializeField]
        private int _actionPoints = 10;
        
        [SerializeField]
        private int _maxActionPoints = 10;
        
        [SerializeField]
        private int _actionPointsRegenPerTurn = 5;

        public int ActionPoints
        {
            get => _actionPoints;
            set => _actionPoints = Mathf.Max(value, 0);
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
    }
}