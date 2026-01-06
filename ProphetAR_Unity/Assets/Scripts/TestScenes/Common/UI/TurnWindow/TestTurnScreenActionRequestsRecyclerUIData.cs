using Swill.Recycler;

namespace ProphetAR
{
    public class TestTurnScreenActionRequestsRecyclerUIData : IRecyclerScrollRectData<long>
    {
        public long Key { get; }
        
        public long ActionNumber { get; }
        
        public GameTurnActionRequest ActionRequest { get; }

        private static int _actionRequestNumber;

        public TestTurnScreenActionRequestsRecyclerUIData(GameTurnActionRequest actionRequest)
        {
            _actionRequestNumber++;
            
            Key = _actionRequestNumber;
            ActionNumber = _actionRequestNumber;
            
            ActionRequest = actionRequest;
        }
    }
}