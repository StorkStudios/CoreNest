using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    private RangeBoundariesDrawer rangeBoundariesDrawer = new RangeBoundariesDrawer();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty minProperty;
        SerializedProperty maxProperty;
        if (property.type == "RangeBoundariesFloat")
        {
            minProperty = property.FindPropertyRelative("Min");
            maxProperty = property.FindPropertyRelative("Max");
        }
        else if (property.type == "Vector2")
        {
            minProperty = property.FindPropertyRelative("x");
            maxProperty = property.FindPropertyRelative("y");
        }
        else
        {
            GUI.Label(position, $"Use of MinMaxRange is not supperted with {property.type}.\nOnly RangeBoundariesFloat or Vector2 are allowed.");
            return;
        }
        float minValue = minProperty.floatValue;
        float maxValue = maxProperty.floatValue;
        EditorGUI.LabelField(position, label);
        position.xMin += EditorGUIUtility.labelWidth;
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.MinMaxSlider(position, GUIContent.none, ref minValue, ref maxValue, (attribute as MinMaxRangeAttribute).Min, (attribute as MinMaxRangeAttribute).Max);
        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;
        position.y += EditorGUIUtility.singleLineHeight;
        if (property.type == "RangeBoundariesFloat")
        {
            rangeBoundariesDrawer.OnGUI(position, property, GUIContent.none);
        }
        else
        {
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }
}
