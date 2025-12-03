using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(GridSection))]
    public class GridSectionCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Create New Section"))
            {
                
            }
        }
    }
}