#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(GridSection))]
    public class GridSectionCustomEditor : UnityEditor.Editor
    {
        private Vector2 _createNewSectionDimensions;
        private Vector2 _setCellDimensions;

        private void OnEnable()
        {
            _createNewSectionDimensions = new Vector2(2f, 2f);
            _setCellDimensions = serializedObject.FindProperty("_cellDimensions").vector2Value;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GridSection gridSection = (GridSection) target;
            
            EditorGUILayout.LabelField("Snap", EditorStyles.boldLabel);
            if (GUILayout.Button("Snap Sections To Self"))
            {
                gridSection.SnapSectionsToSelf();
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clear Section", EditorStyles.boldLabel);
            if (GUILayout.Button("Clear Section"))
            {
                gridSection.ClearSection();
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Cell Dimensions", EditorStyles.boldLabel);
            _setCellDimensions = EditorGUILayout.Vector2Field("Set Cell Dimensions", _setCellDimensions);
            if (GUILayout.Button("Set Cell Dimensions"))
            {
                gridSection.SetCellDimensions(_setCellDimensions);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Create New Section", EditorStyles.boldLabel);
            _createNewSectionDimensions = EditorGUILayout.Vector2Field("Section Dimensions", _createNewSectionDimensions);
            if (GUILayout.Button("Create New Section"))
            {
                gridSection.CreateNewSection(_createNewSectionDimensions);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add Rows/Columns", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Up Row"))
            {
                gridSection.AddUpRow();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Add Down Row"))
            {
                gridSection.AddDownRow();
                EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("Add Left Column"))
            {
                gridSection.AddLeftCol();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Add Right Column"))
            {
                gridSection.AddRightCol();
                EditorUtility.SetDirty(target);
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Remove Rows/Columns", EditorStyles.boldLabel); 
            if (GUILayout.Button("Remove Up Row"))
            {
                gridSection.RemoveUpRow();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Remove Down Row"))
            {
                gridSection.RemoveDownRow();
                EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("Remove Left Column"))
            {
                gridSection.RemoveLeftCol();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Remove Right Column"))
            {
                gridSection.RemoveRightCol();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif