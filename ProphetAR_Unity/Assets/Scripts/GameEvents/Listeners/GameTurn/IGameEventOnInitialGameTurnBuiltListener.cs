using System.Collections.Generic;

namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOnInitialGameTurnBuilt))]
    public interface IGameEventOnInitialGameTurnBuiltListener : IGameEventWithTypedDataListener<IGameEventOnInitialGameTurnBuiltListener, IEnumerable<GameTurnActionRequest>>
    {
        
    }
}