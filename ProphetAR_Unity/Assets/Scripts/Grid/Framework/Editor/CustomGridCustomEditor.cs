#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(CustomGrid))]
    public class CustomGridCustomEditor : UnityEditor.Editor
    {
        private SerializedProperty _level;
        
        private SerializedProperty _gridDimensions;
        private SerializedProperty _originGridSection;
        private SerializedProperty _savedGrid;
        
        private SerializedProperty _topLeftCoordinate;
        private SerializedProperty _botRightCoordinate;

        private SerializedProperty _showGridPointTypeIndicators;
        private SerializedProperty _showSpawnPoints;
        
        private void OnEnable()
        {
            _level = serializedObject.FindProperty(nameof(_level));
            
            _gridDimensions = serializedObject.FindProperty(nameof(_gridDimensions));
            _originGridSection = serializedObject.FindProperty(nameof(_originGridSection));
            _savedGrid = serializedObject.FindProperty(nameof(_savedGrid));
            
            _topLeftCoordinate = serializedObject.FindProperty(nameof(_topLeftCoordinate));
            _botRightCoordinate = serializedObject.FindProperty(nameof(_botRightCoordinate));

            _showGridPointTypeIndicators = serializedObject.FindProperty(nameof(_showGridPointTypeIndicators));
            _showSpawnPoints = serializedObject.FindProperty(nameof(_showSpawnPoints));
        }

        public override void OnInspectorGUI()
        {
            CustomGrid grid = (CustomGrid) target;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_level);
            EditorGUILayout.PropertyField(_originGridSection);

            // Read-only grid properties
            EditorGUILayout.PropertyField(_gridDimensions);
            EditorGUILayout.PropertyField(_topLeftCoordinate);
            EditorGUILayout.PropertyField(_botRightCoordinate);

            // Toggle grid point type indicators
            bool prevShowGridPointTypeIndicators = _showGridPointTypeIndicators.boolValue;
            EditorGUILayout.PropertyField(_showGridPointTypeIndicators);
            if (_showGridPointTypeIndicators.boolValue != prevShowGridPointTypeIndicators)
            {
                foreach (GridCell gridCell in grid.Cells.Values)
                {
                    if (gridCell.Content != null)
                    {
                        gridCell.Content.DebugShowGridPointTypeIndicator(_showGridPointTypeIndicators.boolValue);
                        EditorUtility.SetDirty(gridCell.Content);
                    }
                }
            }
            
            // Toggle spawn points
            bool prevShowSpawnPoints = _showSpawnPoints.boolValue;
            EditorGUILayout.PropertyField(_showSpawnPoints);
            if (_showSpawnPoints.boolValue != prevShowSpawnPoints)
            {
                foreach (GridCell gridCell in grid.Cells.Values)
                {
                    GridCellCharacterSpawnPoint spawnPoint = gridCell.GetComponentInChildren<GridCellCharacterSpawnPoint>();
                    if (spawnPoint != null)
                    {
                        spawnPoint.ShowDebugIndicator(_showSpawnPoints.boolValue);
                        EditorUtility.SetDirty(spawnPoint);
                    }
                }
            }
            
            // Build the grid coordinates
            EditorGUILayout.Space();
            if (GUILayout.Button("Save Grid"))
            {
                grid.SaveGrid();
                EditorUtility.SetDirty(target);
            }
            
            // Read-only (long) list of saved grid coordinates
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_savedGrid);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif