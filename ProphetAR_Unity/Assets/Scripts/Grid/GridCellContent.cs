using UnityEngine;

namespace ProphetAR
{
    public class GridCellContent : MonoBehaviour
    {
        [SerializeField]
        private GridCell _cell = null;

        public GridCell Cell => _cell;

        public void SetGridCell(GridCell cell)
        {
            _cell = cell;
        }
    }
}