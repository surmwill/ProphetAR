using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenActionRequestsToolbarUI : MonoBehaviour,
        IGameEventOnInitialGameTurnBuiltListener,
        IGameEventOnPostGameTurnListener,
        IGameEventGameTurnActionsModifiedListener
    {
        [SerializeField]
        private TestTurnScreenUI _testTurnScreenUI = null;

        [SerializeField]
        private TestTurnScreenActionRequestsRecyclerUI _actionRequestsRecycler = null;

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
            foreach (GamePlayer player in Level.Current.Players)
            {
                if (bind)
                {
                    player.EventProcessor.AddListenerWithData<IGameEventOnInitialGameTurnBuiltListener, IEnumerable<GameTurnActionRequest>>(this);
                    player.EventProcessor.AddListenerWithData<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>(this);
                    
                    player.EventProcessor.AddListenerWithoutData<IGameEventOnPostGameTurnListener>(this);
                }
                else
                {
                    player.EventProcessor.AddListenerWithData<IGameEventOnInitialGameTurnBuiltListener, IEnumerable<GameTurnActionRequest>>(this);
                    player.EventProcessor.AddListenerWithData<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>(this);
                    
                    player.EventProcessor.AddListenerWithoutData<IGameEventOnPostGameTurnListener>(this);
                }
            }
        }
        
        // Turn built
        void IGameEventWithTypedDataListener<IGameEventOnInitialGameTurnBuiltListener, IEnumerable<GameTurnActionRequest>>.OnEvent(IEnumerable<GameTurnActionRequest> data)
        {
            _actionRequestsRecycler.AppendEntries(data.Select(actionRequest => new TestTurnScreenActionRequestsRecyclerUIData(actionRequest)));
        }

        // Post turn
        void IGameEventWithoutDataListener<IGameEventOnPostGameTurnListener>.OnEvent()
        {
            Clear();
        }

        public void Clear()
        {
            _actionRequestsRecycler.Clear();
        }

        // Turn actions modified
        void IGameEventWithTypedDataListener<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>.OnEvent(GameEventGameTurnActionsModifiedData data)
        {
            if (!data.HasInitialTurnBeenBuilt)
            {
                return;
            }
            
            switch (data.ModificationReason)
            {
                case GameEventGameTurnActionsModifiedData.ModificationType.Added:
                    _actionRequestsRecycler.AppendEntries(new[] { new TestTurnScreenActionRequestsRecyclerUIData(data.ModifiedRequest) });
                    break;

                case GameEventGameTurnActionsModifiedData.ModificationType.Removed:
                    _actionRequestsRecycler.RemoveAtKey(data.ModifiedRequest.RequestNum);
                    break;

                case GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged:
                {
                    int? firstGreaterPriorityIndex = null;
                    for (int i = 0; i < _actionRequestsRecycler.DataForEntries.Count; i++)
                    {
                        TestTurnScreenActionRequestsRecyclerUIData recyclerData = _actionRequestsRecycler.DataForEntries[i];
                        if (recyclerData.ActionRequest.Priority > data.ModifiedRequest.Priority)
                        {
                            firstGreaterPriorityIndex = i;
                            break;
                        }
                    }

                    if (firstGreaterPriorityIndex.HasValue &&
                        ((firstGreaterPriorityIndex.Value == 0 && _actionRequestsRecycler.DataForEntries[0].ActionRequest != data.ModifiedRequest) ||
                        (_actionRequestsRecycler.DataForEntries[firstGreaterPriorityIndex.Value - 1].ActionRequest != data.ModifiedRequest)))
                    {
                        _actionRequestsRecycler.RemoveAtKey(data.ModifiedRequest.RequestNum);
                        _actionRequestsRecycler.InsertAtIndex(firstGreaterPriorityIndex.Value, new TestTurnScreenActionRequestsRecyclerUIData(data.ModifiedRequest));
                    }
                    else if (!firstGreaterPriorityIndex.HasValue && _actionRequestsRecycler.DataForEntries[^1].ActionRequest != data.ModifiedRequest)
                    {
                        _actionRequestsRecycler.RemoveAtKey(data.ModifiedRequest.RequestNum);
                        _actionRequestsRecycler.AppendEntries(new[] { new TestTurnScreenActionRequestsRecyclerUIData(data.ModifiedRequest) });
                    }

                    break;
                }
            }
        }
    }
}