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

            TestCharacterMovement testCharacterMovement = (TestCharacterMovement) target;
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Move To Coordinates"))
            {
                testCharacterMovement.MoveToCoordinates();
            }
            else if (GUILayout.Button("Walk To Coordinates"))
            {
                testCharacterMovement.WalkToCoordinates();
            }
        }
    }
}
#endif