using UnityEngine;

namespace ProphetAR
{
    public class GridCellContent : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private GridCell _cell = null;

        public GridCell Cell => _cell;

        public void SetGridCell(GridCell cell)
        {
            _cell = cell;
            transform.position = cell.Middle;
        }
    }
}