namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOnPreGameTurn))]
    public interface IGameEventOnPreGameTurnListener : IGameEventWithoutDataListener<IGameEventOnPreGameTurnListener>
    {
        
    }
}