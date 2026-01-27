using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenARObjectSelectionToolbarUI : MonoBehaviour, 
        IGameEventShowARObjectSelectionUIListener, 
        IGameEventHideARObjectSelectionUIListener
    {
        [SerializeField]
        private TestTurnScreenUI _testTurnScreenUI = null;
        
        [SerializeField]
        private TestTurnScreenARObjectSelectionRecyclerUI _selectionRecycler = null;

        public void InitLevel()
        {
            BindListeners(true);   
        }

        public void DeInitLevel()
        {
            BindListeners(false);
        }

        private void BindListeners(bool bind)
        {
            if (bind)
            {
                Level.Current.EventProcessor.AddListenerWithData<IGameEventShowARObjectSelectionUIListener, GameEventShowARObjectSelectionUIOptionsData>(this);
                Level.Current.EventProcessor.AddListenerWithoutData<IGameEventHideARObjectSelectionUIListener>(this);
            }
            else
            {
                Level.Current.EventProcessor.RemoveListenerWithData<IGameEventShowARObjectSelectionUIListener>(this);
                Level.Current.EventProcessor.RemoveListenerWithoutData<IGameEventHideARObjectSelectionUIListener>(this);
            }
        }

        // Selection started
        void IGameEventWithTypedDataListener<IGameEventShowARObjectSelectionUIListener, GameEventShowARObjectSelectionUIOptionsData>.OnEvent(GameEventShowARObjectSelectionUIOptionsData data)
        {
            bool foundCancel = false;

            List<TestTurnScreenARObjectSelectionRecyclerUIData> optionsRecyclerData = new List<TestTurnScreenARObjectSelectionRecyclerUIData>();
            foreach (ARObjectSelectionUIOptionData optionData in data.OptionsData)
            {
                optionsRecyclerData.Add(new TestTurnScreenARObjectSelectionRecyclerUIData(optionData));
                foundCancel = foundCancel || optionData.IsCancelOption;
            }

            if (!foundCancel)
            {
                Debug.LogWarning("No cancel option provided!");
            }
                
            
            _selectionRecycler.AppendEntries(optionsRecyclerData);
        }

        // Selection completed or cancelled
        void IGameEventWithoutDataListener<IGameEventHideARObjectSelectionUIListener>.OnEvent()
        {
            Clear();
        }

        public void Clear()
        {
            _selectionRecycler.Clear();
        }
    }
}