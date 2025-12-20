using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public class TestCharacterMovement : MonoBehaviour
    {
        [SerializeField]
        private Level _level = null;

        [SerializeField]
        private Character _characterPrefab = null;

        [SerializeField]
        private Vector2Int _targetCoordinates = Vector2Int.one;

        private Character _character;
        private Coroutine _walkToCoordinatesCoroutine;

        private void Start()
        {
            _character = _level.Grid.InstantiateGridObject(_characterPrefab, _targetCoordinates);
        }
        
        public void MoveToCoordinates()
        {
            if (_walkToCoordinatesCoroutine != null)
            {
                StopCoroutine(_walkToCoordinatesCoroutine);
                _walkToCoordinatesCoroutine = null;
            }
            
            _character.GridTransform.MoveToImmediate(_targetCoordinates);
        }

        public void WalkToCoordinates()
        {
            if (_walkToCoordinatesCoroutine == null)
            {
                _walkToCoordinatesCoroutine = StartCoroutine(WalkToCoordinatesInner());
            }
        }

        private IEnumerator WalkToCoordinatesInner()
        {
            yield return _character.WalkToCoordinates(_targetCoordinates);
            _walkToCoordinatesCoroutine = null;
        }
    }
}