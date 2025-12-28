namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOnGameTurnCompleted))]
    public interface IGameEventOnGameTurnCompletedListener : IGameEventWithoutDataListener<IGameEventOnGameTurnCompletedListener>
    {
        
    }
}