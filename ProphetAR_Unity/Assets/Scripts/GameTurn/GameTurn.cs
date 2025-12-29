using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProphetAR
{
    public class GameTurn
    {
        public delegate void CompleteManualPartOfTurnCallback(bool hasMoreManualRequests);
        
        public int TurnNumber { get; }
        
        public GamePlayer Player { get; }

        public CustomPriorityQueue<GameTurnActionRequest> ActionRequests { get; } = new();
        
        public List<Dictionary<string, object>> SerializedTurnActionsForServer { get; } = new();

        private readonly Level _level;

        // The key is the type of game event that fulfills the action request
        private readonly Dictionary<Type, List<GameTurnActionRequest>> _actionRequestsForFulfillment = new();
        
        private bool _initialBuildComplete;
        
        public GameTurn(Level level, GamePlayer player, int turnNumber)
        {
            _level = level;
            Player = player;
            TurnNumber = turnNumber;
        }

        // Initialization
        public void PreTurn()
        {
            _level.EventProcessor.OnGameEventRaised += OnAllGameEvents;
            Player.EventProcessor.OnGameEventRaised += OnAllGameEvents;
            
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnPreGameTurn());
        }

        // Query what actions the player will need to take this turn. These actions might propagate into further actions
        public void InitialBuild()
        {
            // Anyone else can also add actions to the player through this event
            Player.EventProcessor.RaiseEventWithoutData(new GameEventBuildInitialGameTurn());
            
            // Once we have the initial actions built, we'll update any changes in the list 
            ActionRequests.OnAdded += OnAddedActionRequest;
            ActionRequests.OnRemoved += OnRemovedActionRequest;
            ActionRequests.OnPriorityChanged += OnChangedActionRequestPriority;
            
            // The starting state of the turn is now finalized.
            // (Certain actions might trigger further actions, altering the turn. The turn's state changes can be monitored through the above events)
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnInitialGameTurnBuilt(ActionRequests));
        }
        
        public void CompleteActionRequest(GameTurnActionRequest actionRequest)
        {
            ActionRequests.Remove(actionRequest);
            SerializedTurnActionsForServer.Add(actionRequest.SerializeForServer());
        }
        
        /// <summary>
        /// Called by the player once they've completed all turn actions that require manual input
        /// </summary>
        public IEnumerator CompleteManualPartOfTurnCoroutine(CompleteManualPartOfTurnCallback onComplete = null)
        {
            if (ActionRequests.Any())
            {
                throw new InvalidOperationException("There are still incomplete action requests that must be manually handled by the user");
            }
            
            yield return AutomaticPartOfTurnCoroutine();
            
            // If any automatic actions couldn't be executed they will turn into manual actions. Once those are completed, we'll have to call this method again. This loops indefinitely
            if (!ActionRequests.Any())
            {
                // All automatic actions executed and there are no actions left to perform this turn
                OnComplete();
                onComplete?.Invoke(false);
            }
            
            onComplete?.Invoke(true);
        }

        /// <summary>
        /// The second part of the player's turn is resuming any previous actions they've made that progress over multiple turns.
        /// If these actions cannot step forward, they will turn into a manual action, and the manual part of the user's term is returned to.
        /// </summary>
        private IEnumerator AutomaticPartOfTurnCoroutine()
        {
            List<GameTurnActionOverTurns> cancelledActions = new List<GameTurnActionOverTurns>();
            List<GameTurnActionOverTurns> completedActions = new List<GameTurnActionOverTurns>();
            List<GameTurnActionRequest> manualActionRequests = new List<GameTurnActionRequest>();

            List<IEnumerator> turnCoroutines = new List<IEnumerator>();
            foreach (GameTurnActionOverTurns actionOverTurns in Player.State.ActionsOverTurns.Where(actionOverTurns => actionOverTurns.StartAtTurnNum >= TurnNumber))
            {
                if (!actionOverTurns.Turns.MoveNext())
                {
                    completedActions.Add(actionOverTurns);
                    continue;
                }

                GameTurnActionOverTurnsTurn actionOverTurnsTurn = actionOverTurns.Turns.Current;
                if (actionOverTurnsTurn != null)
                {
                    switch (actionOverTurnsTurn.Operation)
                    {
                        case GameTurnActionOverTurnsTurn.TurnOperation.Coroutine:
                            turnCoroutines.Add(actionOverTurnsTurn.TurnCoroutine);
                            break;
                    
                        case GameTurnActionOverTurnsTurn.TurnOperation.Callback:
                            actionOverTurnsTurn.TurnCallback?.Invoke();
                            break;
                    
                        case GameTurnActionOverTurnsTurn.TurnOperation.ManualActionRequest:
                            cancelledActions.Add(actionOverTurns);
                            manualActionRequests.Add(actionOverTurnsTurn.ManualActionRequest);
                            break;
                    }   
                }
            }
            
            if (turnCoroutines.Count > 0)
            {
                yield return new WaitForAllCoroutines(turnCoroutines);
            }

            foreach (GameTurnActionOverTurns completedAction in completedActions.Concat(cancelledActions))
            {
                Player.State.ActionsOverTurns.Remove(completedAction);
            }

            foreach (GameTurnActionRequest manualActionRequired in manualActionRequests)
            {
                ActionRequests.Enqueue(manualActionRequired);
            }
        }
        
        // Final calculations once the turn's actions are over
        private void OnComplete()
        {
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnGameTurnCompleted());
            OnPostTurn();
        }

        // Cleanup
        private void OnPostTurn()
        {
            _level.EventProcessor.OnGameEventRaised -= OnAllGameEvents;
            Player.EventProcessor.OnGameEventRaised -= OnAllGameEvents;
            
            ActionRequests.OnAdded -= OnAddedActionRequest;
            ActionRequests.OnRemoved -= OnRemovedActionRequest;
            ActionRequests.OnPriorityChanged -= OnChangedActionRequestPriority;
            
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnPostGameTurn());
        }
        
        // Used by AI to complete its turn
        public IEnumerator AIExecuteActionRequestsAutomaticallyCoroutine()
        {
            while (ActionRequests.Any())
            {
                List<IEnumerator> actionCoroutines = new List<IEnumerator>();
                List<GameTurnActionRequest> actionRequests = ActionRequests.ToList();
                
                foreach (GameTurnActionRequest actionRequest in actionRequests)
                {
                    switch (actionRequest.AutomaticExecutionMethod)
                    {
                        case GameTurnActionRequest.AutomaticExecutionType.Coroutine:
                            actionCoroutines.Add(actionRequest.ExecuteAutomaticallyCoroutine);
                            break;
                        
                        case GameTurnActionRequest.AutomaticExecutionType.Action:
                            actionRequest.ExecuteAutomaticallyAction?.Invoke();
                            break;
                    }
                }

                if (actionRequests.Count > 0)
                {
                    yield return new WaitForAllCoroutines(actionCoroutines);   
                }

                foreach (GameTurnActionRequest actionRequest in actionRequests)
                {
                    CompleteActionRequest(actionRequest);
                }
            }
            
            OnComplete();
        }
        
        private void OnAddedActionRequest(CustomPriorityQueueItem<GameTurnActionRequest> addedActionRequestItem)
        {
            GameTurnActionRequest addedActionRequest = addedActionRequestItem.Data;
            if (!_actionRequestsForFulfillment.TryGetValue(addedActionRequest.CompletedByGameEventType, out List<GameTurnActionRequest> requestsToFufill))
            {
                requestsToFufill = new List<GameTurnActionRequest>();
                _actionRequestsForFulfillment[addedActionRequest.CompletedByGameEventType] = requestsToFufill;
            }
            
            requestsToFufill.Add(addedActionRequest);
            
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(
                    new GameEventGameTurnActionsModified(new GameEventGameTurnActionsModifiedData(addedActionRequest, GameEventGameTurnActionsModifiedData.ModificationType.Added)));   
            }
        }
        
        private void OnRemovedActionRequest(CustomPriorityQueueItem<GameTurnActionRequest> removedActionRequestItem, bool fromDequeue)
        {
            GameTurnActionRequest removedActionRequest = removedActionRequestItem.Data;
            if (_actionRequestsForFulfillment.TryGetValue(removedActionRequest.CompletedByGameEventType, out List<GameTurnActionRequest> requestsToFufill))
            {
                requestsToFufill.Remove(removedActionRequest);
                if (requestsToFufill.Count == 0)
                {
                    _actionRequestsForFulfillment.Remove(removedActionRequest.CompletedByGameEventType);
                }
            }
            
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(removedActionRequest, GameEventGameTurnActionsModifiedData.ModificationType.Removed, removedByDequeue:fromDequeue)));   
            }
        }

        private void OnChangedActionRequestPriority(CustomPriorityQueueItem<GameTurnActionRequest> changedActionRequestItem, int prevPriority, int newPriority)
        {
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(changedActionRequestItem.Data, GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged, priorityChanged:(prevPriority, newPriority))));   
            }
        }
        
        private void OnAllGameEvents(GameEvent gameEvent)
        {
            Type gameEventType = gameEvent.GetType();
            if (_actionRequestsForFulfillment.TryGetValue(gameEventType, out List<GameTurnActionRequest> actionRequestsFulfilledByGameEventType))
            {
                for (int i = actionRequestsFulfilledByGameEventType.Count - 1; i >= 0; i--)
                {
                    GameTurnActionRequest actionRequest = actionRequestsFulfilledByGameEventType[i];
                    if (actionRequest.IsCompletedByGameEvent(gameEvent))
                    {
                        actionRequestsFulfilledByGameEventType.RemoveAt(i);
                        ActionRequests.Remove(actionRequest);
                    }
                }
            }
        }
    }
}