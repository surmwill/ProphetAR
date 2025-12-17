using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public class TestCharacterMovement : MonoBehaviour
    {
        [SerializeField]
        private Character _character = null;
        
        [SerializeField]
        private Vector2Int _targetCoordinates = default;
        
        private Coroutine _walkToCoordinatesCoroutine = null;

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
        }
    }
}