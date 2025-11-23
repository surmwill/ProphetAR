using System.IO;
using UnityEngine;

public static class GridPathTestingUtils
{
    public static string GetTestFilePath(string fileName)
    {
        return Path.Combine(Application.dataPath, "GridPathFinding", "Editor", "TestInput", fileName);
    }
}
