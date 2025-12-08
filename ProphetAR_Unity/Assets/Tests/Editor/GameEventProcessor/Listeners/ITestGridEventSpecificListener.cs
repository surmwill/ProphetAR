namespace ProphetAR.Tests.GameEvents
{
    [ListensToGameEventType(typeof(TestGridEventSpecific))]
    public interface ITestGridEventSpecificListener : IGameEventWithTypedDataListener<ITestGridEventSpecificListener, TestGridEventDataSpecific>
    {
        
    }
}