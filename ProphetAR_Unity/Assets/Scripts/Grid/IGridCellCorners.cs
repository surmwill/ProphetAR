using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// World corners of a cell
    /// </summary>
    public interface IGridCellCorners
    {
        public Vector3 BotLeft { get; }
        
        public Vector3 TopLeft { get; }
        
        public Vector3 TopRight { get; }
        
        public Vector3 BotRight { get; }
        
        public Vector3 Middle { get; }
    }
}