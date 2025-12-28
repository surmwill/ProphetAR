using System.Collections.Generic;

namespace ProphetAR
{
    /// <summary>
    /// Performs an action over a series of turns
    /// </summary>
    public abstract class GameTurnActionOverTurns : CustomPriorityQueueItem<GameTurnActionOverTurns>
    {
        public int StartAtTurnNum { get; } 
        
        public IEnumerator<GameTurnActionOverTurnsTurn> Turns => _turns ??= ActionOverTurns();

        protected abstract IEnumerator<GameTurnActionOverTurnsTurn> ActionOverTurns();

        private IEnumerator<GameTurnActionOverTurnsTurn> _turns;
        
        public GameTurnActionOverTurns(int? startAtTurnNum = null)
        {
            // Default start on the next turn
            StartAtTurnNum = startAtTurnNum ?? LevelManager.Instance.CurrLevel.TurnManager.TurnNum + 1;  
        }

        public void Reset()
        {
            _turns = null;
        }
    }
}