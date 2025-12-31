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
        [ReadOnly]
        [SerializeField]
        private int _playerIndex = -1;

        [ReadOnly]
        [SerializeField]
        private CharacterStats _characterStats;
        
        public delegate void OnWalkComplete(bool didStopEarly, GridCellContent stoppedAtCell);

        public GamePlayer Player
        {
            get => Grid.Level.Players[_playerIndex];
            set => _playerIndex = value.Index;
        }

        public CharacterStats CharacterStats
        {
            get => _characterStats;
            set => _characterStats = value;
        }

        public void Initialize(GamePlayer player, CharacterStats characterStats)
        {
            Player = player;
            CharacterStats = characterStats;
        }
        
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
                // Move to the stop
                Vector2Int stopCoordinates = instructionsToNextStop.Target.ToVector2Int();
                yield return GridTransform.MoveToAnimated(stopCoordinates, AnimateMovementToCell);

                // Alert that we're there
                GameEventCharacterStoppedData characterStoppedData = new GameEventCharacterStoppedData(stopCoordinates);
                Level.EventProcessor.RaiseEventWithData(new GameEventCharacterStopped(characterStoppedData));
                
                // Perform any actions at that stop
                foreach (GameEventCharacterStoppedData.StopAction stopAction in characterStoppedData.StopActions)
                {
                    yield return stopAction.ExecuteStopAction?.Invoke();
                    
                    // Possibly we need to stop early
                    if (!stopAction.AfterActionCanProgress)
                    {
                        onComplete?.Invoke(true, GridTransform.CurrentCell);
                        yield break;
                    }
                }
            }
            
            // We reached the target
            onComplete?.Invoke(false, GridTransform.CurrentCell);
        }

        protected virtual IEnumerator AnimateMovementToCell(Transform cellParent, Vector3 localCellPosition)
        {
            Sequence sequence = DOTween.Sequence().Append(
                    DOTween.To(
                        () => GridTransform.transform.position,
                        nextWorldPosition => GridTransform.transform.position = nextWorldPosition,
                        cellParent.TransformPoint(localCellPosition),
                        1f));
            
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