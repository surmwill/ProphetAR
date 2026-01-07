namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventCharacterOutOfActions))]
    public interface IGameEventCharacterOutOfActions : IGameEventWithTypedDataListener<IGameEventCharacterOutOfActions, Character>
    {
        
    }
}