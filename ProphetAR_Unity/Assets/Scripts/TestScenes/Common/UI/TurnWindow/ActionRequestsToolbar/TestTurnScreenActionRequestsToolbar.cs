using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenActionRequestsToolbar : MonoBehaviour,
        IGameEventOnInitialGameTurnBuiltListener,
        IGameEventOnPostGameTurnListener,
        IGameEventGameTurnActionsModifiedListener
    {
        [SerializeField]
        private TestTurnScreenUI _testTurnScreenUI = null;

        [SerializeField]
        private TestTurnScreenActionRequestsRecyclerUI _actionRequestsRecycler = null;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => _testTurnScreenUI.Level.IsInitialized);
            BindListeners(true);
        }

        private void OnDestroy()
        {
            BindListeners(false);
        }

        private void BindListeners(bool bind)
        {
            foreach (GamePlayer player in _testTurnScreenUI.Level.Players)
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
            _actionRequestsRecycler.Clear();
        }

        // Turn actions modified
        void IGameEventWithTypedDataListener<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>.OnEvent(GameEventGameTurnActionsModifiedData data)
        {
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