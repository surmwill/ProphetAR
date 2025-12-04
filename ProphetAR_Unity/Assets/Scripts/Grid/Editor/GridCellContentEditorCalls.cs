#if UNITY_EDITOR
using UnityEngine;

namespace ProphetAR
{
    public partial class GridCellContent
    {
        private void EditorStart()
        {   
            if (_cell != null && ApplicationUtils.IsEditMode)
            {
                _cell.EditorOnCellDimensionsChanged += EditorOnCellDimensionsChanged;
            }
        }

        private void EditorOnDestroy()
        {
            if (_cell != null)
            {
                _cell.EditorOnCellDimensionsChanged -= EditorOnCellDimensionsChanged;
            }
        }

        private void EditorSetGridCell(GridCell currCell, GridCell newCell)
        {
            if (currCell != null)
            {
                currCell.EditorOnCellDimensionsChanged -= EditorOnCellDimensionsChanged;   
            }
            
            if (newCell != null && ApplicationUtils.IsEditMode)
            {
                newCell.EditorOnCellDimensionsChanged += EditorOnCellDimensionsChanged;
            }
        }
        
        private void EditorOnCellDimensionsChanged(Vector2 newDimensions)
        {
            transform.position = _cell.Middle;
        }
    }
}
#endif