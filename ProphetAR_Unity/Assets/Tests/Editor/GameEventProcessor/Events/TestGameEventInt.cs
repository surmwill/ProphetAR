namespace ProphetAR.Tests.GameEvents
{
    public class TestGameEventInt : GameEventWithTypedData<int>
    {
        public TestGameEventInt(int data) : base(data)
        {
        }
    }
}