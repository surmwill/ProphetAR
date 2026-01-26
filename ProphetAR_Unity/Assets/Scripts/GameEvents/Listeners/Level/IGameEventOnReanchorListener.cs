namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOnReanchor))]
    public interface IGameEventOnReanchorListener : IGameEventWithoutDataListener<IGameEventOnReanchorListener>
    {
        
    }
}