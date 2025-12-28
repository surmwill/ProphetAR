namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventBuildInitialGameTurn))]
    public interface IGameEventBuildInitialGameTurnListener : IGameEventWithoutDataListener<IGameEventBuildInitialGameTurnListener>
    {
        
    }
}