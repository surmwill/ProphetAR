using GridOperations;

namespace ProphetAR
{
    /// <summary>
    /// A grid point is a char on the underlying 2d char array we use for our grid. Different chars represent different things.
    /// We use this to map the cells in unity to their representation in the array.
    /// </summary>
    public enum GridPointType
    {
        [HasGridPointChar(GridPoints.Clear)]
        Clear = 0,
        
        [HasGridPointChar(GridPoints.Obstacle)]
        Obstacle = 1,
        
        // Clear is a placeholder. This can be any digit (amount of movement steps required if it's not 1)
        [HasGridPointChar(GridPoints.Clear)]
        ModificationStep = 2
    }
}