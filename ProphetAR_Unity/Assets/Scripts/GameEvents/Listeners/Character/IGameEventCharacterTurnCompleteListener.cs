namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventCharacterTurnComplete))]
    public interface IGameEventCharacterTurnCompleteListener : IGameEventWithTypedDataListener<IGameEventCharacterTurnCompleteListener, Character>
    {
        
    }
}