namespace ProphetAR.Tests.GameEvents
{
    [ListensToGameEventType(typeof(TestGameEventNoDataCopy))]
    public interface ITestGameEventNoDataCopyListener : IGameEventWithoutDataListener<ITestGameEventNoDataCopyListener>
    {
        
    }
}