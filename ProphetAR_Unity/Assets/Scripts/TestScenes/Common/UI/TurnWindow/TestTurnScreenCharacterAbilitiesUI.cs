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
        
        public bool Shown { get; private set; }
        
        private const float ShowTime = 1.4f;
        private const float HideTime = 0.4f;
        
        private readonly List<TestTurnScreenCharacterAbilityUI> _characterAbilities = new();
        
        private bool _firstShow;
        
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
                
                if (_firstShow)
                {
                    _firstShow = false;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_abilitiesParent);   
                }
            }
            
            if (Shown)
            {
                return;
            }
            Shown = true;

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

            if (!Shown)
            {
                return;
            }
            Shown = false;
            
            _hideSequence = DOTween.Sequence()
                .Append(transform.DOLocalMoveY(0, HideTime))
                .OnKill(() =>
                {
                    if (!clear)
                    {
                        return;
                    }

                    foreach (TestTurnScreenCharacterAbilityUI ability in _characterAbilities)
                    {
                        Destroy(ability.gameObject);
                    }
                    
                    _characterAbilities.Clear();
                });
        }
    }
}