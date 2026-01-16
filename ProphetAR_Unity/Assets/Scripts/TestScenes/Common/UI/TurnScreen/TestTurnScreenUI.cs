using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestTurnScreenUI : MonoBehaviour, IGameEventOnPreGameTurnListener
    {
        [SerializeField]
        private Level _level = null;

        [SerializeField]
        private TMP_Text _currPlayerText = null;

        [SerializeField]
        private TMP_Text _currTurnText = null;

        [SerializeField]
        private Button _completeManualPartOfTurnButton = null;

        public Level Level => _level;
        
        private GameTurn CurrTurn => _level.TurnManager.CurrTurn;

        private void Awake()
        {
            _completeManualPartOfTurnButton.onClick.AddListener(CompleteManualPartOfTurn);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => _level.IsInitialized);
            BindListeners(true);
        }
        
        private void OnDestroy()
        {
            BindListeners(false);
            _completeManualPartOfTurnButton.onClick.RemoveListener(CompleteManualPartOfTurn);
        }
        
        // Button to go to the next turn
        private void CompleteManualPartOfTurn()
        {
            _completeManualPartOfTurnButton.enabled = false;
            StartCoroutine(CurrTurn.CompleteManualPartOfTurnCoroutine(hasMoreManualRequests => _completeManualPartOfTurnButton.enabled = hasMoreManualRequests));
        }
        
        private void BindListeners(bool bind)
        {
            foreach (GamePlayer player in _level.Players)
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
    }
}