using System.Collections.Generic;

namespace ProphetAR
{
    public class MultiGameTurnAction : CustomPriorityQueueItem<MultiGameTurnAction>
    {
        // Automatically executes actions over a number of turns. If the action cannot execute, an action request is returned for the user to manually deal with
        public IEnumerator<GameTurnActionRequest> ExecuteNextTurn { get; }
    }
}