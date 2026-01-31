using UnityEngine;

namespace ProphetAR
{
    public interface IGridCellContentCoordinatesChangedListener
    {
        void OnCoordinatesChanged(Vector2Int newCoordinates);
    }
}