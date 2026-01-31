using UnityEngine;

namespace ProphetAR
{
    public interface IGridCellContentDimensionsChangedListener
    {
        public void OnDimensionsChanged(Vector2 newDimensions);
    }
}