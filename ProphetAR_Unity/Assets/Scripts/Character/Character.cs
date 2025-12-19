using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    public class Character : GridObject, IGridContentSelfPositioner
    {
        public delegate void OnWalkComplete(bool didStopEarly, GridCellContent stoppedAtCell);
        
        public IEnumerator WalkToCoordinates(Vector2Int targetCoordinates, OnWalkComplete onComplete = null)
        {
            if (targetCoordinates == GridTransform.Coordinates)
            {
                Debug.LogWarning("Walking to the same coordinates we're already on");
                yield break;
            }
            
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
                yield return GridTransform.MoveToAnimated(instructionsToNextStop.Target.ToVector2Int(), AnimateMovementToCell);

                // This should be a game event
                bool canProgressToNextStop = false;
                yield return GridTransform.CurrentCell.OnCharacterStoppedHere(canProgress => canProgressToNextStop = canProgress);

                if (!canProgressToNextStop && targetCoordinates != GridTransform.Coordinates)
                {
                    onComplete?.Invoke(true, GridTransform.CurrentCell);
                    yield break;
                }
            }
            
            onComplete?.Invoke(false, GridTransform.CurrentCell);
        }

        protected virtual IEnumerator AnimateMovementToCell(Transform cellParent, Vector3 localCellPosition)
        {
            Sequence sequence = DOTween.Sequence().Append(
                    DOTween.To(
                        () => GridTransform.transform.position,
                        nextWorldPosition => Grid.transform.position = nextWorldPosition,
                        cellParent.TransformPoint(localCellPosition),
                        2.4f));
            
            yield return sequence.WaitForCompletion();
        }

        public virtual Transform GetCellParent(GridCellContent cell)
        {
            return cell.CharactersRoot;
        }

        public virtual Vector3 GetLocalPositionInCell(GridCellContent cell)
        {
           return Vector3.zero;
        }
    }
}