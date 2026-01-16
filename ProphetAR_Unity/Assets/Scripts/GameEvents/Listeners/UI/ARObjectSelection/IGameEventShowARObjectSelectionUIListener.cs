namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventShowARObjectSelectionUI))]
    public interface IGameEventShowARObjectSelectionUIListener : IGameEventWithTypedDataListener<IGameEventShowARObjectSelectionUIListener, GameEventShowARObjectSelectionUIOptionsData>
    {
        
    }
}