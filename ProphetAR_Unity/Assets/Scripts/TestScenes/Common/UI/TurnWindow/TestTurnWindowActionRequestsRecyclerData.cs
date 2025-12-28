using Swill.Recycler;

namespace ProphetAR
{
    public class TestTurnWindowActionRequestsRecyclerData : IRecyclerScrollRectData<long>
    {
        public long Key { get; }
        
        public long ActionNumber { get; }
        
        public GameTurnActionRequest ActionRequest { get; }

        public TestTurnWindowActionRequestsRecyclerData(long actionNumber, GameTurnActionRequest actionRequest)
        {
            Key = actionNumber;
            ActionNumber = actionNumber;
            
            ActionRequest = actionRequest;
        }
    }
}