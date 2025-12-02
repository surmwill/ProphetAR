using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace ProphetAR
{
    [CustomEditor(typeof(GridCell))]
    public class GridCellCustomEditor : UnityEditor.Editor
    {
        private SerializedProperty _editorNextGridCellSpawnLocationProperty;
        private SerializedProperty _dimensionsProperty;
        private SerializedProperty _coordinatesProperty;

        private void OnEnable()
        {
            _editorNextGridCellSpawnLocationProperty = serializedObject.FindProperty("_editorNextGridCellSpawnLocation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_editorNextGridCellSpawnLocationProperty);
            if (GUILayout.Button("Copy To Left Cell"))
            {
            }
            else if (GUILayout.Button("Copy To Right Cell"))
            {
            }
            else if (GUILayout.Button("Copy To Up Cell"))
            {
            }
            else if (GUILayout.Button("Copy To Down Cell"))
            {
            }
            else if (GUILayout.Button("Add to Grid"))
            {
            }
            else if (GUILayout.Button("Rebuild Grid"))
            {
            }
        }
    }
}

#endif