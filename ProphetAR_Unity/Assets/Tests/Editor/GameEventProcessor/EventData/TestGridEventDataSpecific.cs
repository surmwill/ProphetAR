namespace ProphetAR.Tests.GameEvents
{
    public class TestGridEventDataSpecific : TestGridEventDataBase
    {
        public override void Raise(GameEventProcessor gameEventProcessor)
        {
            gameEventProcessor.RaiseEventWithData(new TestGridEventSpecific(this));
        }
    }
}