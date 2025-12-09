using System.Collections.Generic;

namespace ProphetAR
{
    public interface IMultiGameTurnAction
    {
        public const int DefaultPriority = int.MaxValue;
        
        public int? Priority { get; }
        
        public IEnumerator<bool> ExecuteNextTurn { get; }

        public void OnComplete();

        public void OnCancelled();
    }
}