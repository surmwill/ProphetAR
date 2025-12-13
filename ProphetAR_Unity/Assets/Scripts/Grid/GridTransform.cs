using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Anything that needs to be located on the grid has this
    /// </summary>
    public class GridTransform
    {
        public CustomGrid Grid { get; private set; }
        
        public Vector2 Coordinates { get; private set; }

        public void Initialize(CustomGrid grid, Vector2 coordinates)
        {
            Grid = grid;
            Coordinates = coordinates;
        }
        
        
        
    }
}