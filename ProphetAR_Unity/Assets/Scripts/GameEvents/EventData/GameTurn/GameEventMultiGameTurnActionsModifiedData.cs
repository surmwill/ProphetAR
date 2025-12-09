
namespace ProphetAR
{
    public class GameEventMultiGameTurnActionsModifiedData
    {
        public IMultiGameTurnAction ModifiedMultiAction { get; }
        
        public ModificationType Modified { get; }
        
        public int PrevPriority { get; }
        
        public int CurrPriority { get; }
        
        public GameEventMultiGameTurnActionsModifiedData(IMultiGameTurnAction modifiedMultiAction, ModificationType modificationType, int? prevPriority = null, int? currPriority = null)
        {
            Modified = modificationType;
            ModifiedMultiAction = modifiedMultiAction;
            
            PrevPriority = prevPriority ?? IGameTurnActionRequest.DefaultPriority;
            CurrPriority = currPriority ?? IGameTurnActionRequest.DefaultPriority;
        }
        
        public enum ModificationType
        {
            Added = 0,
            Removed = 1,
            PriorityChanged = 2,
        }
    }
}