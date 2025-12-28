namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventGameTurnActionsModified))]
    public interface IGameEventGameTurnActionsModifiedListener : IGameEventWithTypedDataListener<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>
    {
        
    }
}