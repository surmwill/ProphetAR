using System;
using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public class TestCharacterMovement : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _targetCoordinates = Vector2Int.zero;

        private Character _character;
        private Coroutine _walkToCoordinatesCoroutine;

        private void Awake()
        {
            _character = GetComponentInParent<Character>();
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