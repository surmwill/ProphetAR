using System.Collections.Generic;

namespace ProphetAR
{
    public class GameTurn
    {
        public int TurnNumber { get; }

        public bool HasActionRequests => _actionRequests.Any();
        
        public GamePlayer Player { get; }

        public List<Dictionary<string, object>> SerializedTurnActionsForServer { get; } = new();

        private readonly SmallPriorityQueue<IGameTurnActionRequest, int> _actionRequests = new();

        private bool _initialBuildComplete;
        
        public GameTurn(int turnNumber, GamePlayer player)
        {
            TurnNumber = turnNumber;
            Player = player;
        }

        public void InitialBuild()
        {
            // Listeners will add action requests to the queue. These initial action request might propagate further action requests
            Player.EventProcessor.RaiseEventWithoutData(new GameEventBuildInitialTurn());
            _initialBuildComplete = true;
        }

        public void AddActionRequest(IGameTurnActionRequest actionRequest)
        {
            _actionRequests.Enqueue(actionRequest, actionRequest.Priority ?? IGameTurnActionRequest.DefaultPriority);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventTurnActionsModified(
                    new GameEventTurnActionsModifiedData(actionRequest, GameEventTurnActionsModifiedData.ModificationType.Added)));
            }
        }
        
        public void RemoveActionRequestWithoutCompletion(IGameTurnActionRequest actionRequest)
        {
            _actionRequests.Remove(actionRequest, actionRequest.Priority ?? IGameTurnActionRequest.DefaultPriority);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventTurnActionsModified(
                    new GameEventTurnActionsModifiedData(actionRequest, GameEventTurnActionsModifiedData.ModificationType.Removed)));
            }
        }

        public void ChangeActionRequestPriority(IGameTurnActionRequest actionRequest, int? newPriority)
        {
            int prevPrio = actionRequest.Priority ?? IGameTurnActionRequest.DefaultPriority;
            int newPrio = newPriority ?? IGameTurnActionRequest.DefaultPriority; 
            
            _actionRequests.ChangePriority(actionRequest, prevPrio, newPrio);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventTurnActionsModified(
                    new GameEventTurnActionsModifiedData(actionRequest, GameEventTurnActionsModifiedData.ModificationType.PriorityChanged, prevPrio, newPriority)));
            }
        }
        
        public void CompleteActionRequest(IGameTurnActionRequest actionRequest)
        {
            _actionRequests.Remove(actionRequest, actionRequest.Priority ?? IGameTurnActionRequest.DefaultPriority);
            SerializedTurnActionsForServer.Add(actionRequest.SerializeForServer());
        }
        
        // Used by AI to complete its turn
        public void ExecuteAutomatically()
        {
            while (_actionRequests.Any())
            {
                IGameTurnActionRequest action = _actionRequests.Peek();
                action.ExecuteAutomatically();
                CompleteActionRequest(action);
            }
        }
    }
}