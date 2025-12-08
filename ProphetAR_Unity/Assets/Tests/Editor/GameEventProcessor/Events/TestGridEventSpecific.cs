namespace ProphetAR.Tests.GameEvents
{
    public class TestGridEventSpecific : GameEventWithTypedData<TestGridEventDataSpecific>
    {
        public TestGridEventSpecific(TestGridEventDataSpecific data) : base(data)
        {
        }
    }
}