#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    [CustomEditor(typeof(TestCharacterMovement))]
    public class TestCharacterMovementCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Move To Coordinates"))
            {
                ((TestCharacterMovement) target).MoveToCoordinates();
            }
        }
    }
}
#endif