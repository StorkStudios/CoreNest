using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(SubAssetAttribute))]
    public class SubAssetDrawer : PropertyDrawer
    {
        private static bool IsPropertyTypeSupported(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference &&
                   property.objectReferenceValue is not Component;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsPropertyTypeSupported(property))
            {
                string message = "SubAsset can only be used with UnityEngine.Object that are not UnityEngine.Component";
                GUIContent content = EditorGUIUtility.IconContent("console.warnicon");
                content.text = message;
                FieldInfo field = property.GetFieldInfo();
                content.tooltip = $"{field.DeclaringType.Name}.{field.Name}";
                EditorGUI.LabelField(position, content);
                return;
            }

            Object oldValue = property.objectReferenceValue;

            EditorGUI.BeginChangeCheck();
            
            EditorGUI.PropertyField(position, property);
            
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            Object newValue = property.objectReferenceValue;


        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsPropertyTypeSupported(property))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}