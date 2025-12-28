using UnityEngine;

namespace ProphetAR
{
    public class TestTurnWindowScreenControl : 
        MonoBehaviour, 
        IGameEventOnGameTurnChangedListener,
        IGameEventOnInitialGameTurnBuiltListener,
        IGameEventGameTurnActionsModifiedListener,
    {
        [SerializeField]
        private TestTurnWindowActionRequestsRecycler _actionRequestsRecycler = null;


        public void OnEvent(GameEventOnGameTurnChangedData data)
        {
            throw new System.NotImplementedException();
        }

        public void OnEvent()
        {
            throw new System.NotImplementedException();
        }

        public void OnEvent(GameEventGameTurnActionsModifiedData data)
        {
            throw new System.NotImplementedException();
        }
    }
}