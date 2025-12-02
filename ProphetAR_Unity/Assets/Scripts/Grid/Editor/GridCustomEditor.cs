using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(Grid))]
    public class GridCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            // Rebuild Grid
            if (GUILayout.Button("Rebuild Grid"))
            {
                
            }
            
            // Add origin
            
            // Set cell dimensions
        }
    }
}