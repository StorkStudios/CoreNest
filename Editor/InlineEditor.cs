using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class InlineEditor
    {
        private readonly SerializedObject serializedObject;

        public InlineEditor(SerializedObject objectToDraw)
        {
            serializedObject = objectToDraw;
        }

        public bool DrawInspector()
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(GetHeight()));
            return DrawInspector(rect);
        }

        // Copied from unity source code
        public bool DrawInspector(Rect position)
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    float height = EditorGUI.GetPropertyHeight(iterator);
                    position.yMax = position.yMin + height;
                    EditorGUI.PropertyField(position, iterator, true);
                    position.yMin += height + EditorGUIUtility.standardVerticalSpacing;
                }

                enterChildren = false;
            }

            bool changed = EditorGUI.EndChangeCheck();
            if (changed)
            {
                serializedObject.ApplyModifiedProperties();
            }

            return changed;
        }

        public float GetHeight()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            float result = 0;
            while (iterator.NextVisible(enterChildren))
            {
                result += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;

                enterChildren = false;
            }
            return result;
        }
    }
}