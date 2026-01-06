using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilityUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _abilityName = null;

        public void SetAbility(CharacterAbility ability)
        {
            _abilityName.text = ability.Uid;
        }
    }
}