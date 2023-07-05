using System;
using UnityEditor;
using UnityEngine;

namespace InventorySystem.Scripts.Editor
{
    /// <summary>
    /// Property drawer for SerializableGuid
    ///
    /// Author: Searous (https://github.com/Searous/Unity.SerializableGuid).
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        private const float Y_SEPARATION = 20;
        private float _buttonSize;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Start property draw
            EditorGUI.BeginProperty(position, label, property);

            // Get property
            SerializedProperty serializedGuid = property.FindPropertyRelative("_serializedGuid");

            // Draw label
            position = EditorGUI.PrefixLabel(
                new Rect(position.x, position.y + Y_SEPARATION / 2, position.width, position.height),
                GUIUtility.GetControlID(FocusType.Passive), label);
            position.y -= Y_SEPARATION / 2; // Offsets position so we can draw the label for the field centered

            _buttonSize = position.width / 3; // Update size of buttons to always fit perfectly above the string representation field

            // Buttons
            if (GUI.Button(new Rect(position.xMin, position.yMin, _buttonSize, Y_SEPARATION - 2), "New"))
                serializedGuid.stringValue = Guid.NewGuid().ToString();
            if (GUI.Button(new Rect(position.xMin + _buttonSize, position.yMin, _buttonSize, Y_SEPARATION - 2), "Copy"))
                EditorGUIUtility.systemCopyBuffer = serializedGuid.stringValue;
            if (GUI.Button(new Rect(position.xMin + _buttonSize * 2, position.yMin, _buttonSize, Y_SEPARATION - 2), "Empty"))
                serializedGuid.stringValue = Guid.Empty.ToString();

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            Rect pos = new(position.xMin, position.yMin + Y_SEPARATION, position.width, Y_SEPARATION - 2);
            EditorGUI.PropertyField(pos, serializedGuid, GUIContent.none);

            // End property
            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Field height never changes, so ySep * 2 will always return the proper hight of the field
            return Y_SEPARATION * 2;
        }
    }
}