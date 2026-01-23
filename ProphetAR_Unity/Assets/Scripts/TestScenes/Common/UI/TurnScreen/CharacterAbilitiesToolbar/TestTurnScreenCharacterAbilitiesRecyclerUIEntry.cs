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

        private Character Character => Data.CharacterAbility.Character;
        
        private CharacterAbility Ability => Data.CharacterAbility;
        
        protected override void OnBind(TestTurnScreenCharacterAbilitiesRecyclerUIData entryData)
        {
            _abilityText.text = entryData.CharacterAbility.Uid;
            _abilityButton.onClick.AddListener(ExecuteAbility);
        }

        protected override void OnRecycled()
        {
            _abilityButton.onClick.RemoveListener(ExecuteAbility);
        }

        private void ExecuteAbility()
        {
            Data.CharacterAbility.Execute();   
        }

        private void Update()
        {
            _abilityButton.interactable = Ability.CanExecute();
        }
    }
}