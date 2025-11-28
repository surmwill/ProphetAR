namespace ProphetAR.Tests.GameEvents
{
    [ListensToGameEventType(typeof(TestGameEventNoDataCopy))]
    public interface ITestGameEventNoDataCopyListener : IGameEventWithoutDataListenerExplicit<ITestGameEventNoDataCopyListener>
    {
        
    }
}