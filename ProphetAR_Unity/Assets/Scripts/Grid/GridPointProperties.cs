using System;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Our grid is just a 2D char array. We represent different things on the grid with different chars.
    /// This class translates what the cell is in unity to its character in the array.
    /// </summary>
    [Serializable]
    public class GridPointProperties
    {
        [SerializeField]
        private GridPointType _gridPointType = GridPointType.Clear;
        
        [SerializeField]
        [ReadOnly]
        private char _gridPoint = '\0';

        [SerializeField]
        private int _modificationStep = 1;

        public char GridPoint => _gridPoint;

        public void OnValidate()
        {
            switch (_gridPointType)
            {
                case GridPointType.ModificationStep:
                    _gridPoint = GridPoints.ModificationStepValueToGridPoint(_gridPoint);
                    break;
                
                default:
                    _modificationStep = 1;
                    _gridPoint = _gridPointType.ToGridPointChar();
                    break;
            }
        }
    }
}