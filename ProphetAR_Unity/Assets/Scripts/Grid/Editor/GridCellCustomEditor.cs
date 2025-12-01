using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace ProphetAR
{
    [CustomEditor(typeof(GridCell))]
    [CanEditMultipleObjects]
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
            if (GUILayout.Button("Spawn Adjacent Cell"))
            {
                foreach (var o in targets)
                {
                    
                }
            }
        }

        public void SpawnAdjacentCell(GridCell source, NextGridCellSpawnLocation spawnLocation)
        {
            
        }
        
        
    }
}

#endif