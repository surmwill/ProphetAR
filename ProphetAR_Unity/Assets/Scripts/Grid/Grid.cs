using UnityEngine;

namespace ProphetAR
{
    public partial class Grid : MonoBehaviour
    {
        [Tooltip("(rows, columns)")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _dimensions = default;

        [SerializeField]
        [ReadOnly]
        private Transform _cellsParent;

        [SerializeField]
        private GridCell _origin = default;

        public Vector2 Dimensions => _dimensions;
    }
}