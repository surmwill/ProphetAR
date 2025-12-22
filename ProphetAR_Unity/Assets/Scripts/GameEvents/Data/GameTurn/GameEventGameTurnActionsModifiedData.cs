
namespace ProphetAR
{
    public class GameEventGameTurnActionsModifiedData
    {
        public GameTurnActionRequest ModifiedRequest { get; }
        
        public ModificationType Modified { get; }
        
        public (int prevPriority, int newPriority)? PriorityChanged { get; }
        
        public bool? RemovedByDequeue { get; }
        
        public GameEventGameTurnActionsModifiedData(
            GameTurnActionRequest modifiedRequest, 
            ModificationType modificationType, 
            (int prevPriority, int newPriority)? priorityChanged = null,
            bool? removedByDequeue = null)
        {
            Modified = modificationType;
            ModifiedRequest = modifiedRequest;

            PriorityChanged = priorityChanged;

            RemovedByDequeue = removedByDequeue;
        }
        
        public enum ModificationType
        {
            Added = 0,
            Removed = 1,
            PriorityChanged = 2,
        }
    }
}