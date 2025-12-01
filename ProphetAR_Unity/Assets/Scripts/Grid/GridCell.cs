using UnityEngine;

namespace ProphetAR
{
    public partial class GridCell : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Grid _grid = null;

        [SerializeField]
        [ReadOnly]
        private Vector2 _coordinates = default;

        [SerializeField]
        [Tooltip("(width, height)")]
        [ReadOnly]
        private Vector2 _dimensions = default;
        
        [SerializeField]
        private NextGridCellSpawnLocation _editorNextGridCellSpawnLocation = default;
        
        public GridCell LeftCell { get; private set; }
        
        public GridCell RightCell { get; private set; }
        
        public GridCell UpCell { get; private set; }
        
        public GridCell DownCell { get; private set; }
    }
}