using System.Linq;
using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnWindowScreenControl : 
        MonoBehaviour, 
        IGameEventOnPreGameTurnListener,
        IGameEventOnInitialGameTurnBuiltListener,
        IGameEventGameTurnActionsModifiedListener,
        IGameEventOnPostGameTurnListener
    {
        [SerializeField]
        private Level _level = null;
        
        [SerializeField]
        private TestTurnWindowActionRequestsRecycler _actionRequestsRecycler = null;

        [SerializeField]
        private TMP_Text _currPlayerText = null;

        private GameTurn CurrTurn => _level.TurnManager.CurrTurn;

        private void Awake()
        {
            foreach (GamePlayer player in _level.Players)
            {
                player.EventProcessor.AddListenerWithoutData<IGameEventOnPreGameTurnListener>(this);
                player.EventProcessor.AddListenerWithoutData<IGameEventOnInitialGameTurnBuiltListener>(this);
                player.EventProcessor.AddListenerWithData<IGameEventOnInitialGameTurnBuiltListener, GameEventGameTurnActionsModifiedData>(this);
                player.EventProcessor.AddListenerWithoutData<IGameEventOnPostGameTurnListener>(this);
            }
        }

        /// <summary>
        /// Pre-turn
        /// </summary>
        void IGameEventWithoutDataListener<IGameEventOnPreGameTurnListener>.OnEvent()
        {
            _currPlayerText.text = $"Player: {CurrTurn.Player.Index.ToString()}";
        }
        
        /// <summary>
        /// Post-turn
        /// </summary>
        void IGameEventWithoutDataListener<IGameEventOnPostGameTurnListener>.OnEvent()
        {
            _actionRequestsRecycler.Clear();
        }
        
        /// <summary>
        /// Initial game turn actions built
        /// </summary>
        void IGameEventWithoutDataListener<IGameEventOnInitialGameTurnBuiltListener>.OnEvent()
        {
            _actionRequestsRecycler.AppendEntries(CurrTurn.ActionRequests.Select(actionRequest => new TestTurnWindowActionRequestsRecyclerData(actionRequest)));
        }
        
        /// <summary>
        /// Game turn actions modified after initial build
        /// </summary>
        void IGameEventWithTypedDataListener<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>.OnEvent(GameEventGameTurnActionsModifiedData data)
        {
            switch (data.ModificationReason)
            {
                case GameEventGameTurnActionsModifiedData.ModificationType.Added:
                    _actionRequestsRecycler.AppendEntries(new [] { new TestTurnWindowActionRequestsRecyclerData(data.ModifiedRequest) });
                    break;
                
                case GameEventGameTurnActionsModifiedData.ModificationType.Removed:
                    _actionRequestsRecycler.RemoveAtKey(FindRecyclerDataForActionRequest(data.ModifiedRequest).Key);
                    break;

                case GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged:
                {
                    int firstGreaterPriorityIndex = -1;
                    for (int i = 0; i < _actionRequestsRecycler.DataForEntries.Count; i++)
                    {
                        TestTurnWindowActionRequestsRecyclerData recyclerData = _actionRequestsRecycler.DataForEntries[i];
                        if (recyclerData.ActionRequest.Priority > data.ModifiedRequest.Priority)
                        {
                            firstGreaterPriorityIndex = i;
                            break;
                        }
                    }

                    if (firstGreaterPriorityIndex != -1 &&
                        (firstGreaterPriorityIndex == 0 && _actionRequestsRecycler.DataForEntries[0].ActionRequest != data.ModifiedRequest) ||
                        (_actionRequestsRecycler.DataForEntries[firstGreaterPriorityIndex - 1].ActionRequest != data.ModifiedRequest))
                    {
                        _actionRequestsRecycler.RemoveAtKey(FindRecyclerDataForActionRequest(data.ModifiedRequest).Key);
                        _actionRequestsRecycler.InsertAtKey(firstGreaterPriorityIndex, new TestTurnWindowActionRequestsRecyclerData(data.ModifiedRequest));
                    }

                    break;
                }
            }
            
            TestTurnWindowActionRequestsRecyclerData FindRecyclerDataForActionRequest(GameTurnActionRequest actionRequest)
            {
                return _actionRequestsRecycler.DataForEntries.FirstOrDefault(recyclerData => recyclerData.ActionRequest == actionRequest);
            }
        }
    }
}