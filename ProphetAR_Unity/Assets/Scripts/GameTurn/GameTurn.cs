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
                    new GameEventGameTurnActionsModifiedData(actionRequest, GameEventGameTurnActionsModifiedData.ModificationType.Added)));
            }
        }
        
        public void RemoveActionRequest(IGameTurnActionRequest actionRequest)
        {
            _actionRequests.Remove(actionRequest, actionRequest.Priority ?? IGameTurnActionRequest.DefaultPriority);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(actionRequest, GameEventGameTurnActionsModifiedData.ModificationType.Removed)));
            }
        }
        
        public void CompleteActionRequest(IGameTurnActionRequest actionRequest)
        {
            RemoveActionRequest(actionRequest);
            SerializedTurnActionsForServer.Add(actionRequest.SerializeForServer());
        }

        public void ChangeActionRequestPriority(IGameTurnActionRequest actionRequest, int? newPriority)
        {
            int prevPrio = actionRequest.Priority ?? IGameTurnActionRequest.DefaultPriority;
            int newPrio = newPriority ?? IGameTurnActionRequest.DefaultPriority; 
            
            _actionRequests.ChangePriority(actionRequest, prevPrio, newPrio);
            if (_initialBuildComplete)
            {
                Player.EventProcessor.RaiseEventWithData(new GameEventTurnActionsModified(
                    new GameEventGameTurnActionsModifiedData(actionRequest, GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged, prevPrio, newPriority)));
            }
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