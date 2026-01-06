using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestTurnScreenCharacterAbilitiesUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _abilitiesParent = null;

        [SerializeField]
        private TestTurnScreenCharacterAbilityUI _abilityPrefab = null;
        
        private readonly List<TestTurnScreenCharacterAbilityUI> _characterAbilities = new();

        private const float ShowTime = 1.4f;
        private const float HideTime = 0.4f;
        
        private Sequence _showSequence;
        private Sequence _hideSequence;
        
        public void Show(IEnumerable<CharacterAbility> addCharacterAbilities = null)
        {
            _hideSequence?.Kill(true);
            
            if (addCharacterAbilities != null)
            {
                foreach (CharacterAbility characterAbility in addCharacterAbilities)
                {
                    TestTurnScreenCharacterAbilityUI characterAbilityUI = Instantiate(_abilityPrefab, _abilitiesParent);
                    characterAbilityUI.SetAbility(characterAbility);
                    _characterAbilities.Add(characterAbilityUI);
                }
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(_abilitiesParent);
            }

            if (_characterAbilities.Count == 0)
            {
                return;
            }

            float height = ((RectTransform) _characterAbilities[0].transform).rect.height;
            _showSequence = DOTween.Sequence().Append(transform.DOLocalMoveY(height, ShowTime));
        }

        public void Hide(bool clear = true)
        {
            _showSequence?.Kill();

            _hideSequence = DOTween.Sequence().Append(transform.DOLocalMoveY(0, HideTime));
        }
    }
}