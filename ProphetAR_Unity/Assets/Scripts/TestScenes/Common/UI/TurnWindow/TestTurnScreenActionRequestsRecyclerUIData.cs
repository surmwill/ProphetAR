using Swill.Recycler;

namespace ProphetAR
{
    public class TestTurnScreenActionRequestsRecyclerUIData : IRecyclerScrollRectData<long>
    {
        public long Key { get; }
        
        public GameTurnActionRequest ActionRequest { get; }

        public TestTurnScreenActionRequestsRecyclerUIData(GameTurnActionRequest actionRequest)
        {
            Key = actionRequest.RequestNum;
            ActionRequest = actionRequest;
        }
    }
}