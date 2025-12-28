using UnityEngine;

namespace ProphetAR
{
    public class TestTurnWindowScreenControl : 
        MonoBehaviour, 
        IGameEventOnGameTurnChangedListener,
        IGameEventOnInitialGameTurnBuiltListener,
        IGameEventGameTurnActionsModifiedListener
    {
        [SerializeField]
        private TestTurnWindowActionRequestsRecycler _actionRequestsRecycler = null;

        void IGameEventWithTypedDataListener<IGameEventOnGameTurnChangedListener, GameEventOnGameTurnChangedData>.OnEvent(GameEventOnGameTurnChangedData data)
        {
            
        }

        void IGameEventWithoutDataListener<IGameEventOnInitialGameTurnBuiltListener>.OnEvent()
        {
            
        }

        void IGameEventWithTypedDataListener<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>.OnEvent(GameEventGameTurnActionsModifiedData data)
        {
            
        }
    }
}