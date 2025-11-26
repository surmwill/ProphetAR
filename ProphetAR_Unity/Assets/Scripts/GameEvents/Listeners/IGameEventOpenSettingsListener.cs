namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventOpenSettings))]
    public interface IGameEventOpenSettingsListener : IGameEventWithDataListener
    {
        
    }
}