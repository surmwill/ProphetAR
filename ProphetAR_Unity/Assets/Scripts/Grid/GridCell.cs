using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// A transform representing the bottom left corner of the grid cell
    /// </summary>
    public partial class GridCell : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Grid _grid = null;

        [SerializeField]
        [ReadOnly]
        private Vector2 _coordinates;

        public Vector2 Coordinates => _coordinates;
        
        public GridCell LeftCell { get; private set; }
        
        public GridCell RightCell { get; private set; }
        
        public GridCell UpCell { get; private set; }
        
        public GridCell DownCell { get; private set; }
        
        public void SetData(GridCellData data)
        {
            
        }

        public static GridCell Create(Vector3 botLeftCornerWorldPosition, Vector2 dimensions)
        {
            GridCell cell = new GameObject("Cell (Uninitialized)", typeof(GridCell)).GetComponent<GridCell>();
            cell._dimensions = dimensions;
            
            cell._grid = FindFirstObjectByType<Grid>();
            cell.transform.SetParent(cell._grid.CellsParent);
            
            cell.transform.SetPositionAndRotation(botLeftCornerWorldPosition, Quaternion.identity);
            cell.transform.localScale = Vector3.one;
            
            return cell;
        }
    }
}