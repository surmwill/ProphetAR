namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventFireballStrike))]
    public interface IGameEventFireballStrikeListener : IGameEventWithTypedDataListener<IGameEventFireballStrikeListener, GameEventFireballStrikeData>
    {
    }
}