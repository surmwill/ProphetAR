using System.Collections.Generic;
using System.Linq;

namespace ProphetAR
{
    public class GameTurn
    {
        public int TurnNumber { get; }
        
        public GamePlayer Player { get; }

        public List<Dictionary<string, object>> SerializedTurnActionsForServer { get; } = new();

        private readonly CustomPriorityQueue<GameTurnActionRequest, int> _actionRequests = new();
        private readonly HashSet<IMultiGameTurnAction> _processedMultiGameTurnActions = new(); 

        private bool _initialBuildComplete;
        
        public GameTurn(int turnNumber, GamePlayer player)
        {
            TurnNumber = turnNumber;
            Player = player;
        }

        public void PreTurn()
        {
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnPreGameTurn());
        }

        public void InitialBuild()
        {
            // Listeners will add action requests to the queue. These initial action request might propagate further action requests
            Player.EventProcessor.RaiseEventWithoutData(new GameEventBuildInitialGameTurn());
            _initialBuildComplete = true;
        }

        public void AddActionRequest(GameTurnActionRequest actionRequest)
        {
            _actionRequests.Enqueue(actionRequest, actionRequest.Priority ?? GameTurnActionRequest.DefaultPriority);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(actionRequest, GameEventGameTurnActionsModifiedData.ModificationType.Added)));
            }
        }
        
        public void RemoveActionRequest(GameTurnActionRequest actionRequest)
        {
            _actionRequests.Remove(actionRequest, actionRequest.Priority ?? GameTurnActionRequest.DefaultPriority);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(actionRequest, GameEventGameTurnActionsModifiedData.ModificationType.Removed)));
            }
        }
        
        public void CompleteActionRequest(GameTurnActionRequest actionRequest)
        {
            RemoveActionRequest(actionRequest);
            SerializedTurnActionsForServer.Add(actionRequest.SerializeForServer());
        }

        public void ChangeActionRequestPriority(GameTurnActionRequest actionRequest, int? newPriority)
        {
            int prevPrio = actionRequest.Priority ?? GameTurnActionRequest.DefaultPriority;
            int newPrio = newPriority ?? GameTurnActionRequest.DefaultPriority; 
            
            _actionRequests.ChangePriority(actionRequest, prevPrio, newPrio);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventGameTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(actionRequest, GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged, prevPrio, newPriority)));
            }
        }
        
        /// <summary>
        /// The first part of the player's turn is dealing with new actions that have arisen and need to be completed
        /// </summary>
        public void OnUserCompletedManualPartOfTurn()
        {
            UserExecuteAutomaticPartOfTurn();
            
            // Did any automatic actions fail and turn into manual actions?
            if (!_actionRequests.Any())
            {
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
        
        public void OnComplete()
        {
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnGameTurnCompleted());
            Player.EventProcessor.RaiseEventWithoutData(new GameEventOnPostGameTurn());
        }

        
        // Used by AI to complete its turn
        public void AIExecuteActionRequestsAutomatically()
        {
            while (_actionRequests.Any())
            {
                GameTurnActionRequest action = _actionRequests.Peek();
                action.ExecuteAutomatically();
                CompleteActionRequest(action);
            }
        }
    }
}