namespace ProphetAR.Tests.GameEvents
{
    [ListensToGameEventType(typeof(TestGameEventObject))]
    public interface ITestGameEventObjectListener : IGameEventWithTypedDataListener<ITestGameEventObjectListener, TestObject>
    {
        
    }
}