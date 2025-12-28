namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOnGameTurnChanged))]
    public interface IGameEventOnGameTurnChangedListener : IGameEventWithTypedDataListener<IGameEventOnGameTurnChangedListener, GameEventOnGameTurnChangedData>
    {
        
    }
}