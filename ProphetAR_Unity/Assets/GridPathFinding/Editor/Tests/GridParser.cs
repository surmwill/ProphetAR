using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GridPathFinding.Editor
{
    public static class GridParser
    {
        public static List<char[,]> ParseGridsFromFile(string filePath, IEnumerable<char> ignorePoints = null)
        {
            HashSet<char> ignore = new HashSet<char>(ignorePoints ?? Array.Empty<char>());
        
            List<char[,]> grids = new List<char[,]>();
            List<string> gridLines = new List<string>();
        
            foreach (string line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    grids.Add(ParseGrid(gridLines, ignore));
                    gridLines.Clear();
                }
                else
                {
                    gridLines.Add(line);
                }
            }

            if (gridLines.Count > 0)
            {
                grids.Add(ParseGrid(gridLines, ignore));
            }

            return grids;
        }
    
        public static char[,] CopyGrid(char[,] grid)
        {
            char[,] copiedGrid = new char[grid.GetLength(0), grid.GetLength(1)];
            Array.Copy(grid, copiedGrid, grid.Length);
            return copiedGrid;
        }

        private static char[,] ParseGrid(List<string> lines, HashSet<char> ignorePoints = null)
        {
            (int gridNumRows, int gridNumCols) = (lines.Count, lines[0].Length);
            char[,] grid = new char[gridNumRows, gridNumCols];

            for (int row = 0; row < gridNumRows; row++)
            {
                string line = lines[row];
                for (int col = 0; col < gridNumCols; col++)
                {
                    grid[row, col] = ignorePoints != null && ignorePoints.Contains(line[col]) ? GridPoints.DEBUG_PRINT_CLEAR : line[col];
                }
            }

            return grid;
        }
    }
}