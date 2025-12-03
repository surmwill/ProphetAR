using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(GridSection))]
    public class GridSectionCustomEditor : UnityEditor.Editor
    {
        private Vector2 _createNewSectionDimensions;
        private Vector2 _setCellDimensions;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GridSection gridSection = (GridSection) target;

            if (GUILayout.Button("Clear Section"))
            {
                gridSection.ClearSection();
            }

            _setCellDimensions = EditorGUILayout.Vector2Field("Set Cell Dimensions", _setCellDimensions);
            if (GUILayout.Button("Set Cell Dimensions"))
            {
                gridSection.SetCellDimensions(_setCellDimensions);
            }

            _createNewSectionDimensions = EditorGUILayout.Vector2Field("New Section Dimensions", _createNewSectionDimensions);
            if (GUILayout.Button("Create New Section"))
            {
                gridSection.CreateNewSection(_createNewSectionDimensions);
            }

            EditorGUILayout.LabelField("Add Rows/Columns", EditorStyles.boldLabel);
            if (GUILayout.Button("Add Up Row"))
            {
                gridSection.AddUpRow();
            }

            if (GUILayout.Button("Add Down Row"))
            {
                gridSection.AddDownRow();
            }
            
            if (GUILayout.Button("Add Left Column"))
            {
                gridSection.AddLeftCol();
            }

            if (GUILayout.Button("Add Right Column"))
            {
                gridSection.AddRightCol();
            }
            
            EditorGUILayout.LabelField("Remove Rows/Columns", EditorStyles.boldLabel); 
            if (GUILayout.Button("Remove Up Row"))
            {
                gridSection.RemoveUpRow();
            }

            if (GUILayout.Button("Remove Down Row"))
            {
                gridSection.RemoveDownRow();
            }
            
            if (GUILayout.Button("Remove Left Column"))
            {
                gridSection.RemoveLeftCol();
            }

            if (GUILayout.Button("Remove Right Column"))
            {
                gridSection.RemoveRightCol();
            }
        }
    }
}