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
                base.OnGUI(position, property, label);
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
            if (property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight + (editor != null ? editor.GetHeight() + EditorGUIUtility.standardVerticalSpacing : 0);
            }
            return EditorGUIUtility.singleLineHeight;
        }
    }
}