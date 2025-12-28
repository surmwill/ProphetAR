using System.Collections;

namespace ProphetAR
{
    /// <summary>
    /// Performs an action over a series of turns
    /// </summary>
    public abstract class GameTurnActionOverTurns : CustomPriorityQueueItem<GameTurnActionOverTurns>
    {
        public abstract IEnumerator ActionOverTurnsCoroutine { get; }

        public int StartAtTurnNum { get; } 
        
        public GameTurnActionOverTurns(int? startAtTurnNum = null)
        {
            // Default start on the next turn. Normally a multi-turn action will execute its first step as a manual user
            // action to clear the associated action request. Next turn the same action won't need to be raised again as
            // it's now being handled by the multi-turn action
            StartAtTurnNum = startAtTurnNum ?? LevelManager.Instance.CurrLevel.TurnManager.TurnNum + 1;  
        }
    }
}