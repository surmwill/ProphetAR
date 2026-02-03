using System;
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

        private bool _executingAutomaticPartOfTurn;
        
        private void Awake()
        {
            _completeManualPartOfTurnButton.onClick.AddListener(CompleteManualPartOfTurn);
            
            ShowARObjectSelectionToolbar(false);
            
            Level.RegisterLevelLifecycleListener(this);
        }

        private void OnDestroy()
        { 
            _completeManualPartOfTurnButton.onClick.RemoveListener(CompleteManualPartOfTurn);
            Level.UnregisterLevelLifecycleListener(this);
        }

        private void Update()
        {
            if (Level.Current == null || !Level.Current.HasStarted)
            {
                _completeManualPartOfTurnButton.interactable = false;;
                return;
            }
            
            _completeManualPartOfTurnButton.interactable = !_executingAutomaticPartOfTurn && !Level.Current.TurnManager.CurrTurn.ActionRequests.Any();
        }

        private void InitLevel()
        {
            BindListeners(true);
            
            _actionRequestsToolbar.InitLevel();
            _characterAbilitiesToolbar.InitLevel();
            _arObjectSelectionToolbar.InitLevel();
        }

        private void DeInitLevel()
        {
            BindListeners(false);
            
            _actionRequestsToolbar.DeInitLevel();
            _characterAbilitiesToolbar.DeInitLevel();
            _arObjectSelectionToolbar.DeInitLevel();
        }
        
        // Button to go to the next turn
        private void CompleteManualPartOfTurn()
        {
            _executingAutomaticPartOfTurn = true;
            StartCoroutine(Level.Current.TurnManager.CurrTurn.TryCompleteManualPartOfTurnCoroutine(generatedMoreManualRequests =>
            {
                _executingAutomaticPartOfTurn = false;
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
                Level.Current.EventProcessor.RemoveListenerWithData<IGameEventShowARObjectSelectionUIListener, GameEventShowARObjectSelectionUIOptionsData>(this);
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
            _currPlayerText.text = $"Player: {Level.Current.TurnManager.CurrTurn.Player.Index.ToString()}";
            _currTurnText.text = $"Turn: {Level.Current.TurnManager.CurrTurn.TurnNumber.ToString()}";
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

        public void Clear()
        {
            _actionRequestsToolbar.Clear();
            _characterAbilitiesToolbar.Clear();
            _arObjectSelectionToolbar.Clear();
            
            ShowARObjectSelectionToolbar(false);
        }

        public void OnLevelLifecycleChanged(LevelLifecycleState lifecycleState, Level prevLevel, Level currLevel)
        {
            if (lifecycleState == LevelLifecycleState.Initialized)
            {
               InitLevel();
            }
            else if (lifecycleState == LevelLifecycleState.Destroyed)
            {
                DeInitLevel();
                Clear();
            }
        }
    }
}