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
            if (GUILayout.Button("Parse Grid From Origin"))
            {
                
            }
        }
    }
}