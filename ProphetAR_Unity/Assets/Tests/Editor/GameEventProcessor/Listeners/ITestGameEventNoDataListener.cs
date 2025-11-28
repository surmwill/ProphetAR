namespace ProphetAR.Tests.GameEvents
{
    [ListensToGameEventType(typeof(TestGameEventNoData))]
    public interface ITestGameEventNoDataListener : IGameEventWithoutDataListenerExplicit<ITestGameEventNoDataListener>
    {
        
    }
}