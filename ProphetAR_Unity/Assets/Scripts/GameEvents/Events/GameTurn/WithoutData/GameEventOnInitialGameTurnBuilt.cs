using System.Collections.Generic;

namespace ProphetAR
{
    public class GameEventOnInitialGameTurnBuilt : GameEventWithoutData
    {
        public IEnumerable<GameTurnActionRequest> StartingActionRequests { get; }
        
        public GameEventOnInitialGameTurnBuilt(IEnumerable<GameTurnActionRequest> startingActionRequests)
        {
            StartingActionRequests = startingActionRequests;
        }
    }
}