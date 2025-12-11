
namespace ProphetAR
{
    public class GameEventGameTurnActionsModifiedData
    {
        public GameTurnActionRequest ModifiedRequest { get; }
        
        public ModificationType Modified { get; }
        
        public int PrevPriority { get; }
        
        public int CurrPriority { get; }
        
        public GameEventGameTurnActionsModifiedData(GameTurnActionRequest modifiedRequest, ModificationType modificationType, int? prevPriority = null, int? currPriority = null)
        {
            Modified = modificationType;
            ModifiedRequest = modifiedRequest;
            
            PrevPriority = prevPriority ?? modifiedRequest.Priority;
            CurrPriority = currPriority ?? modifiedRequest.Priority;
        }
        
        public enum ModificationType
        {
            Added = 0,
            Removed = 1,
            PriorityChanged = 2,
        }
    }
}