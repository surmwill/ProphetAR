namespace ProphetAR.Tests.GameEvents
{
    [ListensToGameEventType(typeof(TestGameEventObjectCopy))]
    public interface ITestGameEventObjectCopyListener : IGameEventWithTypedDataListener<ITestGameEventObjectCopyListener, TestObject>
    {
        
    }
}