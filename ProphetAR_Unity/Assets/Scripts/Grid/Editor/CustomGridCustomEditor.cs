using System;
using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(CustomGrid))]
    public class CustomGridCustomEditor : UnityEditor.Editor
    {
        private SerializedProperty _gridDimensions;
        private SerializedProperty _originGridSection;
        private SerializedProperty _savedGrid;
        private SerializedProperty _minCoordinate;
        private SerializedProperty _maxCoordinate;
        
        private void OnEnable()
        {
            _gridDimensions = serializedObject.FindProperty(nameof(_gridDimensions));
            _originGridSection = serializedObject.FindProperty(nameof(_originGridSection));
            _savedGrid = serializedObject.FindProperty(nameof(_savedGrid));
            
            _minCoordinate = serializedObject.FindProperty(nameof(_minCoordinate));
            _maxCoordinate = serializedObject.FindProperty(nameof(_maxCoordinate));
        }

        public override void OnInspectorGUI()
        {
            CustomGrid grid = (CustomGrid) target;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_gridDimensions);
            EditorGUILayout.PropertyField(_originGridSection);

            EditorGUILayout.PropertyField(_minCoordinate);
            EditorGUILayout.PropertyField(_maxCoordinate);
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Save Grid"))
            {
                grid.SaveGrid();
                EditorUtility.SetDirty(target);
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_savedGrid);

            serializedObject.ApplyModifiedProperties();
        }
    }
}