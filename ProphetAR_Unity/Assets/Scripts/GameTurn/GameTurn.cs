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
        
        private readonly HashSet<IMultiGameTurnAction> _processedMultiGameTurnActions = new();

        private readonly Dictionary<Type, GameTurnActionRequest> _actionRequestsForFulfillment = new();
        
        private bool _initialBuildComplete;
        
        public GameTurn(Level level, GamePlayer player, int turnNumber)
        {
            _level = level;
            Player = player;
            TurnNumber = turnNumber;
        }

        public void OnGameEvent(GameEvent gameEvent)
        {
            
        }

        // Initialization
        public void PreTurn()
        {
            _level.EventProcessor.OnGameEventRaised += OnGameEvent;
            Player.EventProcessor.OnGameEventRaised += OnGameEvent;
            
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnPreGameTurn());
        }

        // Query what actions the player will need to take this turn. These actions might propagate into further actions
        public void InitialBuild()
        {
            // Query standard actions for player
            
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
        /// The first part of the player's turn is dealing with any new turn actions
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
            List<IMultiGameTurnAction> cancelledActions = new List<IMultiGameTurnAction>();
            List<IMultiGameTurnAction> completedActions = new List<IMultiGameTurnAction>();
                
            foreach (IMultiGameTurnAction multiGameTurnAction in Player.State.MultiTurnActions.Where(multiGameTurnAction => _processedMultiGameTurnActions.Add(multiGameTurnAction)))
            {
                if (!multiGameTurnAction.ExecuteNextTurn.MoveNext())
                {
                    completedActions.Add(multiGameTurnAction);
                }

                if (!multiGameTurnAction.ExecuteNextTurn.Current)
                {
                    cancelledActions.Add(multiGameTurnAction);
                }
            }

            foreach (IMultiGameTurnAction multiGameTurnAction in cancelledActions)
            {
                multiGameTurnAction.OnCancelled();
                Player.State.MultiTurnActions.RemoveMultiTurnAction(multiGameTurnAction);
            }

            foreach (IMultiGameTurnAction multiGameTurnAction in completedActions)
            {
                multiGameTurnAction.OnComplete();
                Player.State.MultiTurnActions.RemoveMultiTurnAction(multiGameTurnAction);
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
            _level.EventProcessor.OnGameEventRaised -= OnGameEvent;
            Player.EventProcessor.OnGameEventRaised -= OnGameEvent;
            
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
                GameTurnActionRequest action = ActionRequests.Peek();
                action.ExecuteAutomatically();
                CompleteActionRequest(action);
            }
        }
        
        private void OnAddedActionRequest(CustomPriorityQueueItem<GameTurnActionRequest> addedActionRequestItem)
        {
            Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                new GameEventGameTurnActionsModifiedData(addedActionRequestItem.Data, GameEventGameTurnActionsModifiedData.ModificationType.Added)));
        }
        
        private void OnRemovedActionRequest(CustomPriorityQueueItem<GameTurnActionRequest> removeActionRequestItem)
        {
            Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                new GameEventGameTurnActionsModifiedData(removeActionRequestItem.Data, GameEventGameTurnActionsModifiedData.ModificationType.Removed)));
        }

        private void OnChangedActionRequestPriority(CustomPriorityQueueItem<GameTurnActionRequest> changedActionRequestItem, int prevPriority, int newPriority)
        {
            Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                new GameEventGameTurnActionsModifiedData(changedActionRequestItem.Data, GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged, prevPriority, newPriority)));
        }
    }
}