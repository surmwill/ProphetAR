using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilitiesToolbar : MonoBehaviour, 
        IGameEventShowCharacterActionsUIListener,
        IGameEventCharacterStatsModifiedListener,
        IGameEventOnPostGameTurnListener
    {
        [SerializeField]
        private TestTurnScreenUI _testTurnScreenUI;

        [SerializeField]
        private TMP_Text _currCharacterText = null;
        
        [SerializeField]
        private TMP_Text _abilityPointsText = null;

        [SerializeField]
        private TestTurnScreenCharacterAbilitiesRecycler _characterAbilitiesRecycler = null;

        private Character _currCharacter;

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
            foreach (GamePlayer gamePlayer in _testTurnScreenUI.Level.Players)
            {
                if (bind)
                {
                    gamePlayer.EventProcessor.AddListenerWithData<IGameEventShowCharacterActionsUIListener, Character>(this);
                    gamePlayer.EventProcessor.AddListenerWithData<IGameEventCharacterStatsModifiedListener, GameEventCharacterStatsModifiedData>(this);
                
                    gamePlayer.EventProcessor.AddListenerWithoutData<IGameEventOnPostGameTurnListener>(this);
                }
                else
                {
                    gamePlayer.EventProcessor.RemoveListenerWithData<IGameEventShowCharacterActionsUIListener>(this);
                    gamePlayer.EventProcessor.RemoveListenerWithData<IGameEventCharacterStatsModifiedListener>(this);
                
                    gamePlayer.EventProcessor.RemoveListenerWithoutData<IGameEventOnPostGameTurnListener>(this);
                }   
            }
        }
        
        // Show the character's abilities
        void IGameEventWithTypedDataListener<IGameEventShowCharacterActionsUIListener, Character>.OnEvent(Character data)
        {
            _currCharacter = data;
            
            _characterAbilitiesRecycler.ShowAbilitiesForCharacter(data);
            
            _abilityPointsText.text = $"Action points: {data.CharacterStats.ActionPoints.ToString()}";
            _currCharacterText.text = _currCharacter.name;
            
            ShowText(true);
        }
        
        // Cleanup
        void IGameEventWithoutDataListener<IGameEventOnPostGameTurnListener>.OnEvent()
        {
            _characterAbilitiesRecycler.Clear();
            _currCharacter = null;
            
            ShowText(false);
        }

        // Show character action points as they decrease
        void IGameEventWithTypedDataListener<IGameEventCharacterStatsModifiedListener, GameEventCharacterStatsModifiedData>.OnEvent(GameEventCharacterStatsModifiedData data)
        {
            if (data.Character == _currCharacter)
            {
                _abilityPointsText.text = $"Action points: {data.CharacterStats.ActionPoints.ToString()}";   
            }
        }

        private void ShowText(bool show)
        {
            _currCharacterText.gameObject.SetActive(show);
            _abilityPointsText.gameObject.SetActive(show);
        }
    }
}