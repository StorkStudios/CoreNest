using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(EditObjectInInspectorAttribute), true)]
    public class EditObjectInInspectorDrawer : PropertyDrawer
    {
        private InlineEditor editor = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                string message = "Edit object in inspector can only be used with UnityEngine.Object";
                GUIContent content = EditorGUIUtility.IconContent("console.warnicon");
                content.text = message;
                FieldInfo field = property.GetFieldInfo();
                content.tooltip = $"{field.DeclaringType.Name}.{field.Name}";
                EditorGUI.LabelField(position, content);
                return;
            }

            Rect labelRect = position;
            labelRect.yMax = EditorGUIUtility.singleLineHeight + labelRect.yMin;

            if (property.objectReferenceValue != null)
            {
                EditorGUI.PropertyField(labelRect, property, new GUIContent(" "), true);
                property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
            }
            else
            {
                EditorGUI.PropertyField(labelRect, property, label, true);
                property.isExpanded = false;
            }

            Rect editorRect = position;
            editorRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                editor ??= new InlineEditor(new SerializedObject(property.objectReferenceValue));
                editor.DrawInspector(editorRect);

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                editor ??= new InlineEditor(new SerializedObject(property.objectReferenceValue));
                height += EditorGUIUtility.standardVerticalSpacing + editor.GetHeight();
            }
            return height;
        }
    }
}