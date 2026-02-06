using UnityEngine;

public static class GridPathTestingUtils
{
    public static string GetTestFilePath(string fileName)
    {
        return $"{Application.dataPath}/GridOperations/Editor/TestInput/{fileName}";
    }
}
