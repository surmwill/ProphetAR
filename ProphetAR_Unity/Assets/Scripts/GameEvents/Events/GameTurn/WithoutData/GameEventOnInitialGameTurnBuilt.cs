using System.Collections.Generic;

namespace ProphetAR
{
    public class GameEventOnInitialGameTurnBuilt : GameEventWithTypedData<IEnumerable<GameTurnActionRequest>>
    {
        public GameEventOnInitialGameTurnBuilt(IEnumerable<GameTurnActionRequest> data) : base(data)
        {
        }
    }
}