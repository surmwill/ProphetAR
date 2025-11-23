using UnityEngine;

public static class GridPathTestingUtils
{
    public static string GetTestFilePath(string fileName)
    {
        return $"{Application.dataPath}/GridPathFinding/Editor/TestInput/{fileName}";
    }
}
