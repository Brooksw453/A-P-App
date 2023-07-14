using ReliableSolutions.Unity.Common.Utilities;
using UnityEditor;
using UnityEngine;

namespace ReliableSolutions.Unity.Common.Editor.PropertyDrawer
{
    /// <summary>
    /// Defines the Look of a <see cref="Boolean3"/> in the Unity Editor.
    /// It makes sure the three values are shown in one single row instead of three.
    /// </summary>
    [CustomPropertyDrawer(typeof(Boolean3))]
    public class Boolean3Drawer : UnityEditor.PropertyDrawer
    {

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Do not indent child fields
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var rectWidth = position.width / 3f;
            var xRect = new Rect(position.x + rectWidth * 0, position.y, rectWidth, position.height);
            var yRect = new Rect(position.x + rectWidth * 1, position.y, rectWidth, position.height);
            var zRect = new Rect(position.x + rectWidth * 2, position.y, rectWidth, position.height);

            var x = property.FindPropertyRelative("x");
            x.boolValue = EditorGUI.ToggleLeft(xRect, "X", x.boolValue);
            var y = property.FindPropertyRelative("y");
            y.boolValue = EditorGUI.ToggleLeft(yRect, "Y", y.boolValue);
            var z = property.FindPropertyRelative("z");
            z.boolValue = EditorGUI.ToggleLeft(zRect, "Z", z.boolValue);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
