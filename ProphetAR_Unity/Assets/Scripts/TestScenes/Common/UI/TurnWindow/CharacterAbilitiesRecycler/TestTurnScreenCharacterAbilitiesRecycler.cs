using System;
using System.Collections.Generic;
using System.Linq;
using Swill.Recycler;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilitiesRecycler : 
        RecyclerScrollRect<string, TestTurnScreenCharacterAbilitiesRecyclerUIData>,
        IGameEventShowCharacterActionsUIListener,
        IGameEventOnGameTurnChangedListener
    {
        [SerializeField]
        private CanvasGroup _canvasGroup = null;

        private Character _currCharacter;

        private readonly HashSet<Character> _hasAbilitiesInProgress = null;
        
        // Showing actions for different character
        void IGameEventWithTypedDataListener<IGameEventShowCharacterActionsUIListener, Character>.OnEvent(Character data)
        {
            if (data == _currCharacter)
            {
                return;
            }
            _currCharacter = data;
            
            Clear();
            AppendEntries(_currCharacter.Abilities.Select(ability => new TestTurnScreenCharacterAbilitiesRecyclerUIData(ability)));
        }

        // New turn
        void IGameEventWithTypedDataListener<IGameEventOnGameTurnChangedListener, GameEventOnGameTurnChangedData>.OnEvent(GameEventOnGameTurnChangedData data)
        {
            _currCharacter = null;
            _hasAbilitiesInProgress.Clear();
            Clear();
        }

        public void ExecuteAbility(CharacterAbility ability)
        {
            if (ability.Character != _currCharacter)
            {
                throw new InvalidOperationException("Attempting to execute ability of a character other than the currently referenced one");
            }
            
            
        }
    }
}