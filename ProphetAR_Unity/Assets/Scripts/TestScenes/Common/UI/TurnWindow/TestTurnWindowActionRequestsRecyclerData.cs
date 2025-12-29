using Swill.Recycler;

namespace ProphetAR
{
    public class TestTurnWindowActionRequestsRecyclerData : IRecyclerScrollRectData<long>
    {
        public long Key { get; }
        
        public long ActionNumber { get; }
        
        public GameTurnActionRequest ActionRequest { get; }

        private static int _actionRequestNumber;

        public TestTurnWindowActionRequestsRecyclerData(GameTurnActionRequest actionRequest)
        {
            _actionRequestNumber++;
            
            Key = _actionRequestNumber;
            ActionNumber = _actionRequestNumber;
            
            ActionRequest = actionRequest;
        }
    }
}