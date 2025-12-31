using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public class GameTurn
    {
        public delegate void CompleteManualPartOfTurnCallback(bool hasMoreManualRequests);
        
        public int TurnNumber { get; }
        
        public GamePlayer Player { get; }

        public CustomPriorityQueue<GameTurnActionRequest> ActionRequests { get; } = new();
        
        public List<Dictionary<string, object>> ServerSerializedGameStateChangesForTurn { get; } = new();

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
            ServerSerializedGameStateChangesForTurn.Add(actionRequest.ServerSerializedGameStateChanges());
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
            List<GameTurnActionOverTurns> completedActionsOverTurns = new List<GameTurnActionOverTurns>();
            List<GameTurnActionOverTurns> cancelledActionsOverTurns = new List<GameTurnActionOverTurns>();

            List<GameTurnActionRequest> actionsToExecuteThisTurn = new List<GameTurnActionRequest>();
            List<GameTurnActionRequest> postExecutionManualActionsRequired = new List<GameTurnActionRequest>();
            
            foreach (GameTurnActionOverTurns actionOverTurns in Player.State.ActionsOverTurns.Where(actionOverTurns => actionOverTurns.StartAtTurnNum >= TurnNumber))
            {
                if (!actionOverTurns.Turns.MoveNext())
                {
                    completedActionsOverTurns.Add(actionOverTurns);
                    continue;
                }

                GameTurnActionOverTurnsTurn actionOverTurnsTurn = actionOverTurns.Turns.Current;
                if (actionOverTurnsTurn == null)
                {
                    continue;
                }

                if (actionOverTurnsTurn.RequiresManualAction)
                {
                    cancelledActionsOverTurns.Add(actionOverTurns);
                    continue;
                }

                if (actionOverTurnsTurn.TurnActionRequests?.Count > 0)
                {
                    actionsToExecuteThisTurn.AddRange(actionOverTurnsTurn.TurnActionRequests);
                }
            }
            
            yield return ExecuteActionRequestsAutomaticallyCoroutine(actionsToExecuteThisTurn);
            
            foreach (GameTurnActionRequest executedAction in actionsToExecuteThisTurn)
            {
                if (executedAction.DidAutomaticExecutionFail)
                {
                    postExecutionManualActionsRequired.Add(executedAction.OnAutomaticExecutionFailedManualAction);    
                }
                
                ServerSerializedGameStateChangesForTurn.Add(executedAction.ServerSerializedGameStateChanges());   
            }

            foreach (GameTurnActionOverTurns completedActionOverTurns in completedActionsOverTurns.Concat(cancelledActionsOverTurns))
            {
                Player.State.ActionsOverTurns.Remove(completedActionOverTurns);
            }

            foreach (GameTurnActionRequest manualActionRequired in postExecutionManualActionsRequired)
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
        public IEnumerator AIExecuteTurnAutomaticallyCoroutine()
        {
            while (ActionRequests.Any())
            {
                List<GameTurnActionRequest> actionRequests = ActionRequests.ToList();
                
                yield return ExecuteActionRequestsAutomaticallyCoroutine(actionRequests);
                
                foreach (GameTurnActionRequest actionRequest in actionRequests)
                {
                    if (actionRequest.DidAutomaticExecutionFail)
                    {
                        Debug.LogWarning($"Automatic execution failed for AI action: {actionRequest.GetType()}. Manual recovery is not available");
                    }
                    
                    CompleteActionRequest(actionRequest);
                }
            }
            
            OnComplete();
        }

        private IEnumerator ExecuteActionRequestsAutomaticallyCoroutine(IEnumerable<GameTurnActionRequest> actionRequests)
        {
            List<IEnumerator> actionCoroutines = new List<IEnumerator>();
            
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
            
            if (actionCoroutines.Count > 0)
            {
                yield return new WaitForAllCoroutines(actionCoroutines);   
            }
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