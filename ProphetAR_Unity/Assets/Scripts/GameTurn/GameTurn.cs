using System.Collections.Generic;

namespace ProphetAR
{
    public class GameTurn
    {
        public GameTurnManager TurnManager { get; }
        
        public int TurnNumber { get; }

        public bool HasActionRequests => _actionRequests.Any();
        
        public string PlayerUid { get; }

        public List<Dictionary<string, object>> SerializedTurnActionsForServer { get; } = new();

        private const int DefaultPriority = int.MaxValue;

        private readonly SmallPriorityQueue<IGameTurnActionRequest, int> _actionRequests = new();

        private bool _initialBuildComplete;
        
        public GameTurn(GameTurnManager turnManager, int turnNumber, string playerUid)
        {
            TurnManager = turnManager;
            TurnNumber = turnNumber;
            PlayerUid = playerUid;
        }

        public void InitialBuild()
        {
            if (TurnManager.Level.LevelState.TurnActionRequestProvidersPerPlayer.TryGetValue(PlayerUid, out List<IGameTurnActionRequestProvider> requestProviders))
            {
                requestProviders.ForEach(requestProvider => AddActionRequest(requestProvider.GetActionRequest()));
            }
            _initialBuildComplete = true;
        }

        public void AddActionRequest(IGameTurnActionRequest actionRequest)
        {
            _actionRequests.Enqueue(actionRequest, actionRequest.Priority ?? DefaultPriority);
            if (_initialBuildComplete)
            {
                // Event for dynamically added/remove action
            }
        }
        
        public void CompleteActionRequest(IGameTurnActionRequest actionRequest)
        {
            _actionRequests.Remove(actionRequest, actionRequest.Priority ?? DefaultPriority);
            SerializedTurnActionsForServer.Add(actionRequest.SerializeForServer());
        }

        public void RemoveActionRequestWithoutCompletion(IGameTurnActionRequest actionRequest)
        {
            _actionRequests.Remove(actionRequest, actionRequest.Priority ?? DefaultPriority);
            if (_initialBuildComplete)
            {
                // Event for dynamically added/remove action
            }
        }

        public void ChangeActionRequestPriority(IGameTurnActionRequest actionRequest, int? newPriority)
        {
            _actionRequests.ChangePriority(actionRequest, actionRequest.Priority ?? DefaultPriority, newPriority ?? DefaultPriority);
            if (_initialBuildComplete)
            {
                // Event for dynamically changed priority
            }
        }
        
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