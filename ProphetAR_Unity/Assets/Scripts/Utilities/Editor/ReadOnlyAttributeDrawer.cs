#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ProphetAR.Editor
{
    /// <summary>
    /// Draws a readonly property in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}
#endif