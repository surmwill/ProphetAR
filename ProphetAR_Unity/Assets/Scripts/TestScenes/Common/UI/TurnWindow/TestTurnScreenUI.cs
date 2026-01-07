using System.Linq;
using Swill.Recycler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestTurnScreenUI : 
        MonoBehaviour, 
        
        // Turn management events
        IGameEventOnPreGameTurnListener,
        IGameEventOnInitialGameTurnBuiltListener,
        IGameEventGameTurnActionsModifiedListener,
        IGameEventOnPostGameTurnListener,
        
        // Turn events
        IGameEventShowCharacterActionsUIListener
    {
        [SerializeField]
        private Level _level = null;
        
        [SerializeField]
        private TestTurnScreenActionRequestsRecyclerUI _actionRequestsRecycler = null;

        [SerializeField]
        private TestTurnScreenCharacterAbilitiesUI _characterAbilities = null;

        [SerializeField]
        private TMP_Text _currPlayerText = null;

        [SerializeField]
        private TMP_Text _currTurnText = null;

        [SerializeField]
        private Button _completeManualPartOfTurnButton = null;

        private GameTurn CurrTurn => _level.TurnManager.CurrTurn;

        private void Awake()
        {
            BindPlayerEvents(true);
            _completeManualPartOfTurnButton.onClick.AddListener(CompleteManualPartOfTurn);
        }
        
        // Button to go to the next turn
        private void CompleteManualPartOfTurn()
        {
            _completeManualPartOfTurnButton.enabled = false;
            StartCoroutine(CurrTurn.CompleteManualPartOfTurnCoroutine(hasMoreManualRequests => _completeManualPartOfTurnButton.enabled = hasMoreManualRequests));
        }
        
        // Action request: a character has action points left to use. Show the UI with their ability options
        void IGameEventWithTypedDataListener<IGameEventShowCharacterActionsUIListener, Character>.OnEvent(Character data)
        {
            _characterAbilities.Show(data.Abilities);
        }
        
        private void OnDestroy()
        {
            BindPlayerEvents(false);
            _completeManualPartOfTurnButton.onClick.RemoveListener(CompleteManualPartOfTurn);
        }
        
        private void BindPlayerEvents(bool bind)
        {
            foreach (GamePlayer player in _level.Players)
            {
                if (bind)
                {
                    // Turn manager events
                    player.EventProcessor.AddListenerWithoutData<IGameEventOnPreGameTurnListener>(this);
                    player.EventProcessor.AddListenerWithoutData<IGameEventOnInitialGameTurnBuiltListener>(this);
                    player.EventProcessor.AddListenerWithData<IGameEventOnInitialGameTurnBuiltListener, GameEventGameTurnActionsModifiedData>(this);
                    player.EventProcessor.AddListenerWithoutData<IGameEventOnPostGameTurnListener>(this);
                
                    // Turn events
                    player.EventProcessor.AddListenerWithData<IGameEventShowCharacterActionsUIListener, Character>(this);   
                }
                else
                {
                    // Turn manager events
                    player.EventProcessor.RemoveListenerWithoutData<IGameEventOnPreGameTurnListener>(this);
                    player.EventProcessor.RemoveListenerWithoutData<IGameEventOnInitialGameTurnBuiltListener>(this);
                    player.EventProcessor.RemoveListenerWithData<IGameEventOnInitialGameTurnBuiltListener>(this);
                    player.EventProcessor.RemoveListenerWithoutData<IGameEventOnPostGameTurnListener>(this);
                
                    // Turn events
                    player.EventProcessor.RemoveListenerWithData<IGameEventShowCharacterActionsUIListener>(this);   
                }
            }
        }

        #region TurnManagementEvents
        
        /// <summary>
        /// Pre-turn
        /// </summary>
        void IGameEventWithoutDataListener<IGameEventOnPreGameTurnListener>.OnEvent()
        {
            _currPlayerText.text = $"Player: {CurrTurn.Player.Index.ToString()}";
            _completeManualPartOfTurnButton.enabled = true;
        }
        
        /// <summary>
        /// Post-turn
        /// </summary>
        void IGameEventWithoutDataListener<IGameEventOnPostGameTurnListener>.OnEvent()
        {
            _characterAbilities.Hide();
            _actionRequestsRecycler.Clear();
        }
        
        /// <summary>
        /// Initial game turn actions built
        /// </summary>
        void IGameEventWithoutDataListener<IGameEventOnInitialGameTurnBuiltListener>.OnEvent()
        {
            _actionRequestsRecycler.AppendEntries(CurrTurn.ActionRequests.Select(actionRequest => new TestTurnScreenActionRequestsRecyclerUIData(actionRequest)));
        }
        
        /// <summary>
        /// Game turn actions modified after initial build
        /// </summary>
        void IGameEventWithTypedDataListener<IGameEventGameTurnActionsModifiedListener, GameEventGameTurnActionsModifiedData>.OnEvent(GameEventGameTurnActionsModifiedData data)
        {
            switch (data.ModificationReason)
            {
                case GameEventGameTurnActionsModifiedData.ModificationType.Added:
                    _actionRequestsRecycler.AppendEntries(new [] { new TestTurnScreenActionRequestsRecyclerUIData(data.ModifiedRequest) });
                    break;
                
                case GameEventGameTurnActionsModifiedData.ModificationType.Removed:
                    _actionRequestsRecycler.RemoveAtKey(data.ModifiedRequest.RequestNum, FixEntries.HorizontalRight);
                    break;

                case GameEventGameTurnActionsModifiedData.ModificationType.PriorityChanged:
                {
                    int firstGreaterPriorityIndex = -1;
                    for (int i = 0; i < _actionRequestsRecycler.DataForEntries.Count; i++)
                    {
                        TestTurnScreenActionRequestsRecyclerUIData recyclerData = _actionRequestsRecycler.DataForEntries[i];
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
                        _actionRequestsRecycler.RemoveAtKey(data.ModifiedRequest.RequestNum);
                        _actionRequestsRecycler.InsertAtKey(firstGreaterPriorityIndex, new TestTurnScreenActionRequestsRecyclerUIData(data.ModifiedRequest));
                    }

                    break;
                }
            }
        }
        
        #endregion
    }
}