namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOnInitialGameTurnBuilt))]
    public interface IGameEventOnInitialGameTurnBuiltListener : IGameEventWithoutDataListener<IGameEventOnInitialGameTurnBuiltListener>
    {
        
    }
}