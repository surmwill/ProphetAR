using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class GameTurn
    {
        public int TurnNumber { get; }
        
        public string Owner { get; }

        public List<Dictionary<string, object>> SerializedTurnActionsForServer { get; } = new();

        private const int DefaultPriority = int.MaxValue;
        
        private readonly SmallPriorityQueue<IGameTurnActionRequest, int> _actionRequests = new();

        private bool _initialBuildComplete;
        
        public GameTurn(int turnNumber, string owner)
        {
            TurnNumber = turnNumber;
            Owner = owner;
        }

        public virtual void OnInitialBuild()
        {
            // Game event which fills up the priority queue with initially know actions we need to take.
            // Other actions might arise as side effects of these.

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

        public void OnTurnComplete()
        {
            // Event for turn completing
        }
    }
}