using UnityEngine;

namespace ProphetAR
{
    public class TestCharacterMovement : MonoBehaviour
    {
        [SerializeField]
        private Character _character = null;
        
        [SerializeField]
        private Vector2Int _targetCoordinates = default;

        public void MoveToCoordinates()
        {
            _character.GridTransform.MoveToImmediate(_targetCoordinates);
        }

        public void PathToCoordinates()
        {
            
        }
    }
}