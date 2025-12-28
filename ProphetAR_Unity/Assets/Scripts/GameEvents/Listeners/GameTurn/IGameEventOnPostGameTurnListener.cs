namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOnPostGameTurn))]
    public interface IGameEventOnPostGameTurnListener : IGameEventWithoutDataListener<IGameEventOnPostGameTurnListener>
    {
        
    }
}