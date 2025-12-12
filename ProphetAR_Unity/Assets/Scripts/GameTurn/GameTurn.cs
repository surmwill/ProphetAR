using System;
using System.Collections.Generic;
using System.Linq;

namespace ProphetAR
{
    public class GameTurn
    {
        public int TurnNumber { get; }
        
        public GamePlayer Player { get; }

        public CustomPriorityQueue<GameTurnActionRequest> ActionRequests { get; } = new();
        
        public List<Dictionary<string, object>> SerializedTurnActionsForServer { get; } = new();

        private readonly Level _level;
        
        private readonly HashSet<MultiGameTurnAction> _processedMultiGameTurnActions = new();

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
            // Query standard actions for player. Add
            
            // Anyone else can also add actions to the player through this event
            Player.EventProcessor.RaiseEventWithoutData(new GameEventBuildInitialGameTurn());
            
            // Once we have the initial actions built, we'll update any changes in the list 
            ActionRequests.OnAdded += OnAddedActionRequest;
            ActionRequests.OnRemoved += OnRemovedActionRequest;
            ActionRequests.OnPriorityChanged += OnChangedActionRequestPriority;
        }
        
        public void CompleteActionRequest(GameTurnActionRequest actionRequest)
        {
            ActionRequests.Remove(actionRequest);
            SerializedTurnActionsForServer.Add(actionRequest.SerializeForServer());
        }
        
        /// <summary>
        /// Called by the player once they've completed all turn actions that require manual input
        /// </summary>
        public void UserSetManualPartOfTurnComplete()
        {
            if (ActionRequests.Any())
            {
                throw new InvalidOperationException("There are still incomplete action requests that must be manually handled by the user");
            }
            
            UserExecuteAutomaticPartOfTurn();
            
            // If any automatic actions couldn't be executed they will turn into manual actions. Once those are completed, we'll have to call this method again. This loops indefinitely
            if (!ActionRequests.Any())
            {
                // All automatic actions executed and there are no actions left to perform this turn
                OnComplete();
            }
        }

        /// <summary>
        /// The second part of the player's turn is resuming any previous actions they've made that progress over multiple turns.
        /// If these actions cannot step forward, they will turn into a manual action, and the manual part of the user's term is returned to.
        /// </summary>
        private void UserExecuteAutomaticPartOfTurn()
        {
            List<MultiGameTurnAction> cancelledActions = new List<MultiGameTurnAction>();
            List<MultiGameTurnAction> completedActions = new List<MultiGameTurnAction>();
            List<GameTurnActionRequest> manualActionsRequired = new List<GameTurnActionRequest>();
            
            foreach (MultiGameTurnAction multiGameTurnAction in Player.State.MultiTurnActions
                         .Select(multiGameTurnActionItem => multiGameTurnActionItem.Data)
                         .Where(multiGameTurnAction => multiGameTurnAction.StartAtTurnNum >= TurnNumber && _processedMultiGameTurnActions.Add(multiGameTurnAction)))
            {
                if (!multiGameTurnAction.ExecuteNextTurn.MoveNext())
                {
                    completedActions.Add(multiGameTurnAction);
                    continue;
                }

                GameTurnActionRequest manualActionRequired = multiGameTurnAction.ExecuteNextTurn.Current;
                if (manualActionRequired != null)
                {
                    cancelledActions.Add(multiGameTurnAction);
                    manualActionsRequired.Add(manualActionRequired);
                }
            }

            foreach (MultiGameTurnAction completedAction in completedActions.Concat(cancelledActions))
            {
                Player.State.MultiTurnActions.Remove(completedAction);
            }

            foreach (GameTurnActionRequest manualActionRequired in manualActionsRequired)
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
        public void AIExecuteActionRequestsAutomatically()
        {
            while (ActionRequests.Any())
            {
                GameTurnActionRequest action = ActionRequests.Peek().Data;
                action.ExecuteAutomatically();
                CompleteActionRequest(action);
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
        
        private void OnRemovedActionRequest(CustomPriorityQueueItem<GameTurnActionRequest> removedActionRequestItem)
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
                    new GameEventGameTurnActionsModifiedData(removedActionRequest, GameEventGameTurnActionsModifiedData.ModificationType.Removed)));   
            }
        }

        private void OnChangedActionRequestPriority(CustomPriorityQueueItem<GameTurnActionRequest> changedActionRequestItem, int prevPriority, int newPriority)
        {
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(changedActionRequestItem.Data, GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged, prevPriority, newPriority)));   
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