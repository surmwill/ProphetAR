using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    public class Character : GridObject
    {
        public IEnumerator WalkToCoordinates(Vector2Int targetCoordinates)
        {
            NavigationInstructionSet instructionSet = GridTransform.GetPathTo(targetCoordinates, Grid.GetGlobalSliceExpensive());
            if (instructionSet == null)
            {
                Debug.LogWarning($"Could not path to coordinates: {targetCoordinates}");
                yield break;
            }
            
            // Raise movement event. Other objects can modify the movement (ex: request stops along the way) before it is performed
            GameEventCharacterMoveData characterMoveData = new GameEventCharacterMoveData(this, instructionSet);
            Level.EventProcessor.RaiseEventWithData(new GameEventCharacterMove(characterMoveData));

            List<NavigationInstructionSet> stops = instructionSet.SplitOnDirectionChanges(characterMoveData.IntermediateStops.Select(coord => coord.ToTuple()));
            foreach (NavigationInstructionSet instructionsToNextStop in stops)
            {
                // Move to the stop, alert the cell when we're there
                yield return GridTransform.MoveToAnimated(instructionsToNextStop.Target.ToVector2Int(), AnimateMovementToParentInCell, GetParentInCell);
                yield return GridTransform.CurrentCell.OnCharacterStoppedHere();
            }
        }
        
        protected virtual Transform GetParentInCell(GridCellContent cellContent)
        {
            return cellContent.transform;
        }

        protected virtual IEnumerator AnimateMovementToParentInCell(Transform parent)
        {
            Sequence sequence = DOTween.Sequence().Append(GridTransform.transform.DOMove(parent.position, 2.5f));
            yield return sequence.WaitForCompletion();
        }
    }
}