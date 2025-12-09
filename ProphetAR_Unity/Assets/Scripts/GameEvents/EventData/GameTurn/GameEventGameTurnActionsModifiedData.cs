
namespace ProphetAR
{
    public class GameEventGameTurnActionsModifiedData
    {
        public IGameTurnActionRequest ModifiedRequest { get; }
        
        public ModificationType Modified { get; }
        
        public int PrevPriority { get; }
        
        public int CurrPriority { get; }
        
        public GameEventGameTurnActionsModifiedData(IGameTurnActionRequest modifiedRequest, ModificationType modificationType, int? prevPriority = null, int? currPriority = null)
        {
            Modified = modificationType;
            ModifiedRequest = modifiedRequest;
            
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