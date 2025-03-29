using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpeedDistanceTimeConfig))]
public class SpeedDistanceTimeConfigDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty mode = property.FindPropertyRelative("mode");
        SerializedProperty speed = property.FindPropertyRelative("speed");
        SerializedProperty distance = property.FindPropertyRelative("distance");
        SerializedProperty time = property.FindPropertyRelative("time");

        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, label, true);
        fieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(fieldRect, mode);
            fieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            int modeIndex = mode.enumValueIndex;
            using (new GUIEnabledDisposable(modeIndex != 0))
            {
                EditorGUI.PropertyField(fieldRect, speed);
                fieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            using (new GUIEnabledDisposable(modeIndex != 1))
            {
                EditorGUI.PropertyField(fieldRect, distance);
                fieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            using (new GUIEnabledDisposable(modeIndex != 2))
            {
                EditorGUI.PropertyField(fieldRect, time);
                fieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            switch (modeIndex)
            {
                case 0:
                    speed.floatValue = distance.floatValue / time.floatValue;
                    break;
                case 1:
                    distance.floatValue = speed.floatValue * time.floatValue;
                    break;
                case 2:
                    time.floatValue = distance.floatValue / speed.floatValue;
                    break;
            }

            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fields = property.isExpanded ? 5 : 1;
        return fields * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) - EditorGUIUtility.standardVerticalSpacing;
    }
}
