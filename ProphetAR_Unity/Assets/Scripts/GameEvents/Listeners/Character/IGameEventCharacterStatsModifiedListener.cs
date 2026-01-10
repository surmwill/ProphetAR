namespace ProphetAR
{
    [ListensToGameEventType(typeof(GameEventCharacterStatsModified))]
    public interface IGameEventCharacterStatsModifiedListener : IGameEventWithTypedDataListener<IGameEventCharacterStatsModifiedListener, GameEventCharacterStatsModifiedData>
    {
        
    }
}