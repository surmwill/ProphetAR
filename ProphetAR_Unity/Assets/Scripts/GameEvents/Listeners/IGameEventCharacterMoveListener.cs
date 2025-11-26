namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventCharacterMove))]
    public interface IGameEventCharacterMoveListener : IGameEventWithTypedDataListener<GameEventCharacterMoveData>
    {
        
    }
}