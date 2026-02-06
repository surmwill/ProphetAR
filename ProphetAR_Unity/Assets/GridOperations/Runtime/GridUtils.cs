using System.Text;

namespace GridOperations
{
    public static class GridUtils
    {
        public static string PrintRawGrid(char[,] grid)
        {
            StringBuilder sb = new StringBuilder();
            
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    char point = grid[x, y];
                    sb.Append(point == GridPoints.Clear ? GridPoints.DEBUG_PRINT_PATH : point); // Null (clear) doesn't show up
                }
            
                sb.AppendLine();
            }
            
            return sb.ToString();
        }
    }
}