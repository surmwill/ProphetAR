using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(CustomGrid))]
    public class GridCustomEditor : UnityEditor.Editor
    {
        private GridCell _snapGridCell;
        private GridCell _snapGridCellTo;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CustomGrid grid = (CustomGrid) target;
            
            if (GUILayout.Button("Save Grid"))
            {
                grid.SaveGrid();   
            }
            
            EditorGUILayout.LabelField("Snap Grid Sections Together",  EditorStyles.boldLabel);
            _snapGridCell = (GridCell) EditorGUILayout.ObjectField("Snap", _snapGridCell, typeof(GridCell), true);
            _snapGridCellTo = (GridCell) EditorGUILayout.ObjectField("Snap To", _snapGridCellTo, typeof(GridCell), true);

            bool canSnap = _snapGridCell != null && _snapGridCellTo !=null && _snapGridCell.ParentGridSection != _snapGridCellTo.ParentGridSection; 
            
            if (GUILayout.Button("Right") && canSnap)
            {
                grid.SnapGridCellsTogether(_snapGridCell, _snapGridCellTo, GridDirection.Right);
            }
            
            if (GUILayout.Button("Left") && canSnap)
            {
                grid.SnapGridCellsTogether(_snapGridCell, _snapGridCellTo, GridDirection.Left);
            }
            
            if (GUILayout.Button("Up") && canSnap)
            {
                grid.SnapGridCellsTogether(_snapGridCell, _snapGridCellTo, GridDirection.Up);
            }
            
            if (GUILayout.Button("Down") && canSnap)
            {
                grid.SnapGridCellsTogether(_snapGridCell, _snapGridCellTo, GridDirection.Down);
            }
        }
    }
}