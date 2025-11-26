namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOpenMainMenu))]
    public interface IGameEventOpenMainMenuListener : IGameEventWithoutDataListener
    {
        
    }
}