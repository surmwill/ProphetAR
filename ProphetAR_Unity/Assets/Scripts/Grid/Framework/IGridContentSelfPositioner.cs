using UnityEngine;

namespace ProphetAR
{
    public interface IGridContentSelfPositioner
    {
        Transform GetCellParent(GridCellContent cell);
        
        Vector3 GetLocalPositionInCell(GridCellContent cell);
    }
}