using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RangeBoundaries<float>))]
[CustomPropertyDrawer(typeof(RangeBoundariesFloat))]
public class RangeBoundariesDrawer : PropertyDrawer
{
    private readonly float minLabelWidth = 26;
    private readonly float maxLabelWidth = 30;
    private readonly float gap = 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;

        if (label != GUIContent.none)
        {
            EditorGUI.LabelField(position, label);
            position.xMin += EditorGUIUtility.labelWidth;
        }
        float width = (position.width + gap) / 2;
        position.xMax -= width + gap;

        EditorGUI.LabelField(position, "Min");
        position.xMin += minLabelWidth;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Min"), GUIContent.none);
        position.xMin -= minLabelWidth;

        position.x += width + gap * 0.75f;

        EditorGUI.LabelField(position, "Max");
        position.xMin += maxLabelWidth;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Max"), GUIContent.none);
        position.xMin -= maxLabelWidth;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
