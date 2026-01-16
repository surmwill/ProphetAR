using UnityEngine;

using static ProphetAR.GameEventShowARObjectSelectionUIOptionsData;

namespace ProphetAR
{
    public class TestTurnScreenARObjectSelectionToolbarUI : MonoBehaviour, IGameEventShowARObjectSelectionUIListener
    {
        [SerializeField]
        private TestTurnScreenARObjectSelectionRecyclerUI _selectionRecycler = null;

        // On start selection
        void IGameEventWithTypedDataListener<IGameEventShowARObjectSelectionUIListener, GameEventShowARObjectSelectionUIOptionsData>.OnEvent(GameEventShowARObjectSelectionUIOptionsData data)
        {
            
            
            foreach (ARObjectSelectionUIOptionData selectionData in data.OptionsData)
            {
                
            }
        }
    }
}