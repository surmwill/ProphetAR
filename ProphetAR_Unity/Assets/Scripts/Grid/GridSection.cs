using UnityEngine;
using UnityEngine.Serialization;

namespace ProphetAR
{
    public partial class GridSection : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Vector2 _gridDimensions = default;

        [SerializeField]
        private Transform _cellsParent = null;
        
        [SerializeField]
        private GameObject _cellPrefab = null;

        [SerializeField]
        private Vector2 _cellDimensions = default;
    }
}