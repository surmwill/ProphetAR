using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilitiesToolbarUI : MonoBehaviour, 
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
        private TestTurnScreenCharacterAbilitiesRecyclerUI _characterAbilitiesRecycler = null;

        private Character _currCharacter;

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
            foreach (GamePlayer gamePlayer in Level.Current.Players)
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
            if (_currCharacter == data)
            {
                return;
            }
            _currCharacter = data;

            _characterAbilitiesRecycler.AppendEntries(data.Abilities.Select(ability => new TestTurnScreenCharacterAbilitiesRecyclerUIData(ability)));
            
            _abilityPointsText.text = $"Action points: {data.CharacterStats.ActionPoints.ToString()}";
            _currCharacterText.text = _currCharacter.name;
        }
        
        // Cleanup
        void IGameEventWithoutDataListener<IGameEventOnPostGameTurnListener>.OnEvent()
        {
            Clear();
        }

        // Show character action points as they decrease
        void IGameEventWithTypedDataListener<IGameEventCharacterStatsModifiedListener, GameEventCharacterStatsModifiedData>.OnEvent(GameEventCharacterStatsModifiedData data)
        {
            if (data.Character == _currCharacter)
            {
                _abilityPointsText.text = $"Action points: {data.CharacterStats.ActionPoints.ToString()}";   
            }
        }

        public void Clear()
        {
            _characterAbilitiesRecycler.Clear();
            _currCharacter = null;
        }
    }
}