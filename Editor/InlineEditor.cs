using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class InlineEditor
    {
        private readonly SerializedObject obj;

        public InlineEditor(Object objectToDraw)
        {
            obj = new SerializedObject(objectToDraw);
        }

        // Copied from unity source code
        public void DrawInspector(Rect position)
        {
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            SerializedProperty iterator = obj.GetIterator();
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

            obj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        public float GetHeight()
        {
            SerializedProperty iterator = obj.GetIterator();
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