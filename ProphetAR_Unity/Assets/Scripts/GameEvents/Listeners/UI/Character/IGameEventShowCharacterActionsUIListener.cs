namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventShowCharacterActionsUI))]
    public interface IGameEventShowCharacterActionsUIListener : IGameEventWithoutDataListener<IGameEventShowCharacterActionsUIListener>
    {
        
    }
}