using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    public class NotNullDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent icon = EditorGUIUtility.IconContent("console.erroricon");

            EditorGUI.PropertyField(position, property, label, property.isExpanded);

            position.x -= 18;
            if (property.objectReferenceValue == null)
            {
                GUI.Label(position, icon);
            }
        }
    }
}