namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventShowCharacterActionsUI))]
    public interface IGameEventShowCharacterActionsUIListener : IGameEventWithTypedDataListener<IGameEventShowCharacterActionsUIListener, Character>
    {
        
    }
}