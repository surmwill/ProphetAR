using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProphetAR.Coroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestTurnScreenUI : MonoBehaviour, 
        ILevelLifecycleListener,
        IGameEventOnPreGameTurnListener, 
        IGameEventOnInitialGameTurnBuiltListener,
        IGameEventShowARObjectSelectionUIListener, 
        IGameEventHideARObjectSelectionUIListener
    {
        [SerializeField]
        private TMP_Text _currPlayerText = null;

        [SerializeField]
        private TMP_Text _currTurnText = null;

        [SerializeField]
        private Button _completeManualPartOfTurnButton = null;

        [SerializeField]
        private TestTurnScreenActionRequestsToolbarUI _actionRequestsToolbar = null;

        [SerializeField]
        private TestTurnScreenCharacterAbilitiesToolbarUI _characterAbilitiesToolbar = null;

        [SerializeField]
        private TestTurnScreenARObjectSelectionToolbarUI _arObjectSelectionToolbar = null;
        
        private GameTurn CurrTurn => Level.Current.TurnManager.CurrTurn;

        private void Awake()
        {
            _completeManualPartOfTurnButton.onClick.AddListener(CompleteManualPartOfTurn);
            ShowARObjectSelectionToolbar(false);
        }

        private IEnumerator Start()
        {
            yield return new WaitForInitializedLevel();
            Initialize();
        }

        private void Initialize()
        {
            BindListeners(true);
            
            _actionRequestsToolbar.Initialize();
            _characterAbilitiesToolbar.Initialize();
            _arObjectSelectionToolbar.Initialize();
        }
        
        private void OnDestroy()
        { 
            _completeManualPartOfTurnButton.onClick.RemoveListener(CompleteManualPartOfTurn);
            BindListeners(false);
        }
        
        // Button to go to the next turn
        private void CompleteManualPartOfTurn()
        {
            StartCoroutine(CurrTurn.TryCompleteManualPartOfTurnCoroutine(generatedMoreManualRequests =>
            {
                _completeManualPartOfTurnButton.enabled = generatedMoreManualRequests;
                if (!generatedMoreManualRequests)
                {
                    StartCoroutine(Level.Current.TurnManager.NextTurnCoroutine());
                }
            }));
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
            
            foreach (GamePlayer player in Level.Current.Players)
            {
                if (bind)
                {
                    player.EventProcessor.AddListenerWithoutData<IGameEventOnPreGameTurnListener>(this);
                }
                else
                {
                    player.EventProcessor.RemoveListenerWithoutData<IGameEventOnPreGameTurnListener>(this);
                }
            }
        }
      
        // Pre-turn
        void IGameEventWithoutDataListener<IGameEventOnPreGameTurnListener>.OnEvent()
        {
            _currPlayerText.text = $"Player: {CurrTurn.Player.Index.ToString()}";
            _currTurnText.text = $"Turn: {CurrTurn.TurnNumber.ToString()}";
            _completeManualPartOfTurnButton.enabled = true;
        }
        
        // Turn built
        void IGameEventWithTypedDataListener<IGameEventOnInitialGameTurnBuiltListener, IEnumerable<GameTurnActionRequest>>.OnEvent(IEnumerable<GameTurnActionRequest> data)
        {
            _completeManualPartOfTurnButton.enabled = !data.Any();
        }

        // Show AR object selection UI
        void IGameEventWithTypedDataListener<IGameEventShowARObjectSelectionUIListener, GameEventShowARObjectSelectionUIOptionsData>.OnEvent(GameEventShowARObjectSelectionUIOptionsData data)
        {
            ShowARObjectSelectionToolbar(true);
        }

        // Hide AR object selection UI
        void IGameEventWithoutDataListener<IGameEventHideARObjectSelectionUIListener>.OnEvent()
        {
            ShowARObjectSelectionToolbar(false);
        }

        // Show selection toolbar instead of character abilities one 
        private void ShowARObjectSelectionToolbar(bool show)
        {
            _arObjectSelectionToolbar.gameObject.SetActive(show);
            _characterAbilitiesToolbar.gameObject.SetActive(!show);
        }

        public void OnLevelLifecycleChanged(LevelLifecycleState lifecycleState, Level prevLevel, Level currLevel)
        {
            if (lifecycleState == LevelLifecycleState.Destroyed)
            {
                _actionRequestsToolbar.Clear();
                _characterAbilitiesToolbar.Clear();
                _arObjectSelectionToolbar.Clear();
            }
        }
    }
}