using System.Collections;

namespace ProphetAR
{
    public interface IMultiGameTurnAction
    {
        public const int DefaultPriority = int.MaxValue;
        
        public int? Priority { get; }
        
        public IEnumerator ExecuteNextTurn();
    }
}