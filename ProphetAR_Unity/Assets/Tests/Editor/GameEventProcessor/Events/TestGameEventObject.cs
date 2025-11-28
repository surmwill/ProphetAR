namespace ProphetAR.Tests.GameEvents
{
    public class TestGameEventObject : GameEventWithTypedData<TestObject>
    {
        public TestGameEventObject(TestObject data) : base(data)
        {
        }
    }
}