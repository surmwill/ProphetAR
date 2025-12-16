using System;
using System.Collections;
using DG.Tweening;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    public class Character : GridObject
    {
        public IEnumerator WalkToCoordinates(Vector2Int targetCoordinates)
        {
            Vector2Int currCoordinates = GridTransform.Coordinates;

            NavigationInstructionSet instructionSet = GridTransform.GetPathTo(targetCoordinates, Grid.GetGlobalSliceExpensive());
            if (instructionSet == null)
            {
                Debug.LogWarning($"Could not path to coordinates: {targetCoordinates}");
                yield return null;
            }
            
            // Game event movement raised. List<NavigationInstructionSet> returned, and a list of Stops and callbacks
            
            // move on navigation instructions set, look for any callbacks on that coordinate. If so, run them . Or cell 
            
            // instruction set gets sliced into horizontal and vertical movements. Stops might be added

            // iterate over these and call on stop in between
            yield return GridTransform.MoveToAnimated(targetCoordinates, AnimateMovementToParentInCell, GetParentInCell);
        }

        protected virtual void OnIntermediateStop(GridCellContent stoppedOnCellContent)
        {
            
        }
        
        protected virtual Transform GetParentInCell(GridCellContent cellContent)
        {
            return cellContent.transform;
        }

        protected virtual IEnumerator AnimateMovementToParentInCell(Transform parent)
        {
            Sequence sequence = DOTween.Sequence().Append(GridTransform.transform.DOMove(parent.position, 1f));
            yield return sequence.WaitForCompletion();
        }
    }
}