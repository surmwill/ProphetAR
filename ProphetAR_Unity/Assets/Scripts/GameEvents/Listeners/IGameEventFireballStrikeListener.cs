namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventFireballStrikeData))]
    public interface IGameEventFireballStrikeListener : IGameEventWithTypedDataListener<GameEventFireballStrikeData>
    {
    }
}