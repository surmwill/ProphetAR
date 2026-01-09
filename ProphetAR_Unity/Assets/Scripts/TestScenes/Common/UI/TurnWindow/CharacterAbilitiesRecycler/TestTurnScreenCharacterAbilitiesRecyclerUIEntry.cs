using System.Collections;
using Swill.Recycler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilitiesRecyclerUIEntry : RecyclerScrollRectEntry<string, TestTurnScreenCharacterAbilitiesRecyclerUIData>
    {
        [SerializeField]
        private TMP_Text _abilityText = null;

        [SerializeField]
        private Button _abilityButton = null;
        
        protected override void OnBind(TestTurnScreenCharacterAbilitiesRecyclerUIData entryData)
        {
            // entryData.CharacterAbility.
        }
    }
}