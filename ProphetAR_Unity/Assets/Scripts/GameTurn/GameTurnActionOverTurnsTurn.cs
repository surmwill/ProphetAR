using System;
using System.Collections;
using System.Collections.Generic;

namespace ProphetAR
{
    /// <summary>
    /// An action performed over multiple turns 
    /// </summary>
    public class GameTurnActionOverTurnsTurn
    {
        /// <summary>
        /// The actions to automatically perform this turn
        /// </summary>
        public List<GameTurnActionRequest> TurnActionRequests { get; }
        
        /// <summary>
        /// If we need to stop the action and handle something manually, this can be returned
        /// </summary>
        public GameTurnActionRequest ManualActionRequired { get; }

        public bool RequiresManualAction => ManualActionRequired != null;

        public static GameTurnActionOverTurnsTurn WithActions(List<GameTurnActionRequest> turnActionRequests)
        {
            return new GameTurnActionOverTurnsTurn(turnActionRequests);
        }

        public static GameTurnActionOverTurnsTurn Cancel(GameTurnActionRequest manualActionRequired)
        {
            return new GameTurnActionOverTurnsTurn(manualActionRequired);
        }

        private GameTurnActionOverTurnsTurn(GameTurnActionRequest manualActionRequired)
        {
            ManualActionRequired = manualActionRequired;
        }

        private GameTurnActionOverTurnsTurn(List<GameTurnActionRequest> turnActionRequests)
        {
            TurnActionRequests = turnActionRequests;
        }
    }
}