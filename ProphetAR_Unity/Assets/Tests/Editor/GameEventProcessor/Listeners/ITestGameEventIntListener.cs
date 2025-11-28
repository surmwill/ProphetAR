namespace ProphetAR.Tests.GameEvents
{
    [ListensToGameEventType(typeof(TestGameEventInt))]
    public interface ITestGameEventIntListener : IGameEventWithTypedDataListener<ITestGameEventIntListener, int>
    {
        
    }
}