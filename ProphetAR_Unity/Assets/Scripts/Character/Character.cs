using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GridPathFinding;
using UnityEngine;

using static GridPathFinding.NavigationInstruction;

namespace ProphetAR
{
    public class Character : GridObject, IGridContentSelfPositioner, IGameEventOnPreGameTurnListener, IGameEventBuildInitialGameTurnListener
    {
        [ReadOnly]
        [SerializeField]
        private int _playerIndex = -1;
        
        [SerializeField]
        private CharacterStats _characterStats;

        private const float MovementTime = 0.5f;
        private const float TurnTime = 0.25f;
        
        public delegate void OnWalkComplete(bool didStopEarly, GridCellContent stoppedAtCell);

        public GamePlayer Player
        {
            get => _playerIndex >= 0 ? Grid.Level.Players[_playerIndex] : null;
            set
            {
                int newPlayerIndex = value?.Index ?? -1;
                if (_playerIndex == newPlayerIndex)
                {
                    return;
                }

                int prevPlayerIndex = _playerIndex;
                _playerIndex = newPlayerIndex;
                
                OnPlayerChange(prevPlayerIndex >= 0 ? Level.Players[prevPlayerIndex] : null, newPlayerIndex >= 0 ? Level.Players[newPlayerIndex] : null);
            }
        }

        public CharacterStats CharacterStats
        {
            get => _characterStats;
            set => _characterStats = value;
        }

        public List<CharacterAbility> Abilities { get; } = new();

        public List<CharacterAbility> CurrentlyExecutingAbilities { get; } = new();

        public bool IsExecutingAbility => CurrentlyExecutingAbilities.Count > 0;

        public void Initialize(GamePlayer player, CharacterStats characterStats)
        {
            Player = player;
            CharacterStats = characterStats;
            
            // Temporarily hard-coded: these should be loaded from the player config and change per character
            Abilities.AddRange(new CharacterAbility[]{ new CharacterAbilityMovement(this), new CharacterAbilityCompleteTurn(this)});
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

        public void CompleteTurn()
        {
            Player.EventProcessor.RaiseEventWithData(new GameEventCharacterTurnComplete(this));
        }

        protected override void OnDestroy()
        {
            Player = null;
            base.OnDestroy();
        }

        #region TurnCallbacks
        
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

        public (NavigationDestinationSet DestinationSet, GridSlice Area) GetMovementArea()
        {
            int maxSteps = _characterStats.ActionPoints;
            GridSlice area = GridSlice.ExtendFromCenter(Grid, GridTransform.Coordinates, maxSteps);
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
            NavigationDirection? lastDirection = null;
            bool stoppedEarly = false;
            
            foreach (NavigationInstructionSet instructionsToNextStop in stops)
            {
                int distanceToStop = instructionsToNextStop.Magnitude;
                
                // Move to the stop
                Vector2Int stopCoordinates = instructionsToNextStop.Target.ToVector2Int();
                NavigationDirection direction = instructionsToNextStop.PathToTarget.First().Direction;

                // Previous stops might have reduced our action points and affected our stop position
                if (CharacterStats.ActionPoints < distanceToStop)
                {
                    int missingDistance = distanceToStop - CharacterStats.ActionPoints;
                    distanceToStop -= missingDistance;
                    
                    switch (direction)
                    {
                        case NavigationDirection.Up:
                            stopCoordinates += Vector2Int.up * missingDistance;
                            break;
                        
                        case NavigationDirection.Down:
                            stopCoordinates -= Vector2Int.up * missingDistance;
                            break;
                        
                        case NavigationDirection.Left:
                            stopCoordinates += Vector2Int.right * missingDistance;
                            break;
                        
                        case NavigationDirection.Right:
                            stopCoordinates -= Vector2Int.right * missingDistance;
                            break;
                    }
                }

                // A new direction means we should turn to face it
                if (!lastDirection.HasValue || lastDirection != direction)
                {
                    Vector3 localRotation = default;
                    
                    switch (direction)
                    {
                        case NavigationDirection.Up:
                            localRotation = Vector3.zero;
                            break;
                        
                        case NavigationDirection.Down:
                            localRotation = Vector3.up * 180f;
                            break;
                        
                        case NavigationDirection.Left:
                            localRotation = Vector3.up * -90f;
                            break;
                        
                        case NavigationDirection.Right:
                            localRotation = Vector3.up * 90f;
                            break;
                    }
                    
                    yield return AnimateMovementTurn(localRotation, TurnTime);
                    
                    lastDirection = direction;
                }
                
                // Move to the stop
                yield return GridTransform.MoveToAnimated(stopCoordinates, AnimateMovementToCell, MovementTime);

                // Alert that we're there
                GameEventCharacterStoppedData characterStoppedData = new GameEventCharacterStoppedData(stopCoordinates);
                Level.EventProcessor.RaiseEventWithData(new GameEventCharacterStopped(characterStoppedData));
                
                // Perform any actions at that stop
                List<GameEventCharacterStoppedData.OnStopAction> stopActions = characterStoppedData.StopActions.ToList();
                foreach (GameEventCharacterStoppedData.OnStopAction stopAction in stopActions)
                {
                    switch (stopAction.ExecutionMethod)
                    {
                        case GameEventCharacterStoppedData.OnStopAction.StopActionExecutionType.Coroutine:
                            yield return stopAction.StopCoroutine;
                            break;
                        
                        case GameEventCharacterStoppedData.OnStopAction.StopActionExecutionType.Action:
                            stopAction.StopAction?.Invoke();
                            break;
                    }

                    // Possibly we need to stop early
                    if (!stopAction.CanProgressAfterStop)
                    {
                        stoppedEarly = true;
                        break;
                    }
                }

                CharacterStats.ModifyActionPoints(this, -distanceToStop);
                if (stoppedEarly || CharacterStats.ActionPoints == 0)
                {
                    break;
                }
            }
            
            // We've reached the target
            onComplete?.Invoke(stoppedEarly, GridTransform.CurrentCell);
        }

        protected virtual IEnumerator AnimateMovementTurn(Vector3 localRotation, float time)
        {
            Sequence sequence = DOTween.Sequence()
                .Append(GridTransform.transform.DOLocalRotate(localRotation, time));

            yield return sequence.WaitForCompletion();
        }

        protected virtual IEnumerator AnimateMovementToCell(Transform cellParent, Vector3 localCellPosition, float time)
        {
            Sequence sequence = DOTween.Sequence().Append(
                    DOTween.To(
                        () => GridTransform.transform.position,
                        nextWorldPosition => GridTransform.transform.position = nextWorldPosition,
                        cellParent.TransformPoint(localCellPosition),
                        time));
            
            yield return sequence.WaitForCompletion();
        }
        
        #endregion
        
        #region CellPositioning
        
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