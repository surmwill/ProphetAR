namespace ProphetAR.Tests.GameEvents
{
    public class TestGameEventObjectCopy : GameEventWithTypedData<TestObject>
    {
        public TestGameEventObjectCopy(TestObject data) : base(data)
        {
        }
    }
}