using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        ShowIfAttribute showIfAttribute = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            showIfAttribute ??= (ShowIfAttribute)attribute;

            Object[] targets = property.serializedObject.targetObjects;
            bool? visible = showIfAttribute.ShouldShow(targets[0]);
            if (!visible.HasValue)
            {
                string message = "Show if can only reference bool field, property or parameterless method.";
                GUIContent content = EditorGUIUtility.IconContent("console.warnicon");
                content.text = message;
                FieldInfo field = property.GetFieldInfo();
                content.tooltip = $"{field.DeclaringType.Name}.{field.Name}";
                EditorGUI.LabelField(position, content);
                return;
            }
            if (targets.All(e => showIfAttribute.ShouldShow(e).Value == visible.Value))
            {
                if (visible.Value)
                {
                    EditorGUI.PropertyField(position, property);
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                using (new EditorGUI.MixedValueScope(true))
                {
                    EditorGUI.PropertyField(position, property);
                }
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            showIfAttribute ??= (ShowIfAttribute)attribute;

            Object[] targets = property.serializedObject.targetObjects;
            bool? visible = showIfAttribute.ShouldShow(targets[0]);
            if (!visible.HasValue)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            if (targets.All(e => showIfAttribute.ShouldShow(e).Value == visible.Value))
            {
                return visible.Value ? EditorGUI.GetPropertyHeight(property) : 0;
            }
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}