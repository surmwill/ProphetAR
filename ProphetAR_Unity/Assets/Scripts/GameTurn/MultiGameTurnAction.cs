using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class MultiGameTurnAction : CustomPriorityQueueItem<MultiGameTurnAction>
    {
        // Automatically executes actions over a number of turns. If the action cannot execute, an action request is returned for the user to manually deal with
        public abstract IEnumerator<GameTurnActionRequest> ExecuteNextTurn { get; }

        public int StartAtTurnNum { get; } 
        
        public MultiGameTurnAction(int? startAtTurnNum = null)
        {
            // Default start on the next turn. Normally a multi-turn action will execute its first step as a manual user
            // action to clear the associated action request. Next turn the same action won't need to be raised again as
            // it's now being handled by the multi-turn action
            StartAtTurnNum = startAtTurnNum ?? LevelManager.Instance.CurrLevel.TurnManager.TurnNum + 1;  
        }
    }
}