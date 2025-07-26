using UnityEngine;
using UnityEditor;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(SerializedPair<,>), true)]
    public class SerializedPairDrawer : PropertyDrawer
    {
        private const float foldoutTriangleWidth = 10;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty value = property.FindPropertyRelative("Value");
            SerializedProperty key = property.FindPropertyRelative("Key");

            EditorGUI.BeginProperty(position, label, property);
            position.yMin += EditorGUIUtility.standardVerticalSpacing;
            position.xMin += foldoutTriangleWidth;
            Rect valuePosition = new Rect(position);
            Rect keyPosition = new Rect(position);

            valuePosition.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            keyPosition.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(valuePosition, value, new GUIContent("Value"), true);
            EditorGUI.PropertyField(keyPosition, key, new GUIContent("Key"));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float keyHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Key"), label);
            float valueHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Value"), label);
            return keyHeight + valueHeight + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}