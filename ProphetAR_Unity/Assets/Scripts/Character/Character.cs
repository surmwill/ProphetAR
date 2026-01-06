using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    public class Character : GridObject, IGridContentSelfPositioner, IGameEventOnPreGameTurnListener, IGameEventBuildInitialGameTurnListener
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
            set
            {
                if (_playerIndex == value.Index)
                {
                    return;
                }

                int prevIndex = _playerIndex;
                _playerIndex = value.Index;
                
                OnPlayerChange(prevIndex >= 0 ? Level.Players[prevIndex] : null, value.Index >= 0 ? Level.Players[value.Index] : null);
            }
        }

        public CharacterStats CharacterStats
        {
            get => _characterStats;
            set => _characterStats = value;
        }
        
        public List<CharacterAbility> Abilities { get; } = new();

        public void Initialize(GamePlayer player, CharacterStats characterStats)
        {
            Player = player;
            CharacterStats = characterStats;
        }

        private void OnPlayerChange(GamePlayer prevPlayer, GamePlayer currPlayer)
        {
            if (prevPlayer != null)
            {
                prevPlayer.EventProcessor.RemoveListenerWithoutData<IGameEventOnPreGameTurnListener>(this);
                prevPlayer.EventProcessor.RemoveListenerWithoutData<IGameEventBuildInitialGameTurnListener>(this);
            }
            
            if (currPlayer != null)
            {
                currPlayer.EventProcessor.AddListenerWithoutData<IGameEventOnPreGameTurnListener>(this);
                currPlayer.EventProcessor.AddListenerWithoutData<IGameEventBuildInitialGameTurnListener>(this);
            }
        }

        protected override void OnDestroy()
        {
            Player = null;
            base.OnDestroy();
        }

        #region Turn_Callbacks
        
        // Pre-turn
        void IGameEventWithoutDataListener<IGameEventOnPreGameTurnListener>.OnEvent()
        {
            UpdateStatsForNewTurn();
        }
        
        // Build turn
        void IGameEventWithoutDataListener<IGameEventBuildInitialGameTurnListener>.OnEvent()
        {
            Level.TurnManager.CurrTurn.ActionRequests.Enqueue(new CharacterActionRequest(this));         
        }

        private void UpdateStatsForNewTurn()
        {
            _characterStats.ActionPoints = Mathf.Min(_characterStats.ActionPoints + _characterStats.ActionPointsRegenPerTurn, _characterStats.MaxActionPoints);
        }
        
        #endregion
        
        #region Movement

        public (NavigationDestinationSet Destinations, GridSlice Area) GetMovementArea()
        {
            int maxSteps = _characterStats.ActionPoints;
            GridSlice area = GridSlice.CreateFromCenter(Grid, GridTransform.Coordinates, maxSteps);
            return (GridTransform.GetPathsFrom(maxSteps, area), area);
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
        
        #endregion
        
        #region Cell_Positioning
        
        public virtual Transform GetCellParent(GridCellContent cell)
        {
            return cell.CharactersRoot;
        }

        public virtual Vector3 GetLocalPositionInCell(GridCellContent cell)
        {
            return Vector3.zero;
        }
        
        #endregion
    }
}