using System.Collections.Generic;
using System.Reflection;

namespace ProphetAR
{
    public static class GridPointTypeExtensions
    {
        private static readonly Dictionary<GridPointType, char> GridPointTypeToChar = new();

        public static char ToGridPointChar(this GridPointType gridPointType)
        {
            if (GridPointTypeToChar.TryGetValue(gridPointType, out char gridPoint))
            {
                return gridPoint;
            }
            
            FieldInfo fieldInfo = typeof(GridPointType).GetField(gridPointType.ToString());
            HasGridPointCharAttribute attribute = fieldInfo.GetCustomAttribute<HasGridPointCharAttribute>(false);

            gridPoint = attribute.GridPoint;
            GridPointTypeToChar[gridPointType] = gridPoint;

            return gridPoint;
        }
    }
}