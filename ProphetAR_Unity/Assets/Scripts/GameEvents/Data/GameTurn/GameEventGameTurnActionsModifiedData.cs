
namespace ProphetAR
{
    public class GameEventGameTurnActionsModifiedData
    {
        // We might not care about modifications of actions while we're still in the process of building the turn's initial action list
        public bool HasInitialTurnBeenBuilt { get; }
        
        public GameTurnActionRequest ModifiedRequest { get; }
        
        public ModificationType ModificationReason { get; }
        
        // Contains information about the priority change if that is indeed the modification reason
        public (int prevPriority, int newPriority)? PriorityChanged { get; }
        
        // Contains information about the removal if that is indeed the modification reason
        public bool? RemovedByDequeue { get; }
        
        public GameEventGameTurnActionsModifiedData(
            bool hasInitialTurnBeenBuilt,
            GameTurnActionRequest modifiedRequest, 
            ModificationType modificationType, 
            (int prevPriority, int newPriority)? priorityChanged = null,
            bool? removedByDequeue = null)
        {
            // General information
            HasInitialTurnBeenBuilt = hasInitialTurnBeenBuilt;
            ModificationReason = modificationType;
            ModifiedRequest = modifiedRequest;

            // Additional information based on modification reason
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