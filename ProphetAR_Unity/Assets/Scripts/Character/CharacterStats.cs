using System;
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

        public void ModifyActionPoints(Character character, int amount)
        {
            ActionPoints = Math.Max(ActionPoints + amount, 0);
            character.Player.EventProcessor.RaiseEventWithData(new GameEventCharacterStatsModified(new GameEventCharacterStatsModifiedData(character, this)));

            if (amount <= 0)
            {
                character.CompleteTurn();
            }
        }
    }
}