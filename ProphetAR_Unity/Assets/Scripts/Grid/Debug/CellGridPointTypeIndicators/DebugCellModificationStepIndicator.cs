using System;
using GridPathFinding;
using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class DebugCellModificationStepIndicator : DebugCellGridPointTypeIndicator
    {
        [SerializeField]
        private TMP_Text _modificationValueText = null;

        public override void SetGridPoint(char gridPoint)
        {
            if (!GridPoints.IsModificationStep(gridPoint, out int numSteps))
            {
                throw new ArgumentException($"Expected a modification step, got point {gridPoint}");
            }
            
            _modificationValueText.text = numSteps.ToString();
        }
    }
}