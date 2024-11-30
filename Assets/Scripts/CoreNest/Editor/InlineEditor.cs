using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InlineEditor
{
    private SerializedObject obj;

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
