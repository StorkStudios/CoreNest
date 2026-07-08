using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(Trigger))]
    [CustomPropertyDrawer(typeof(Trigger<>))]
    public class TriggerDrawer : UnityEventDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect fieldPosition = position;

            fieldPosition.yMax = fieldPosition.yMin + EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(fieldPosition, property.isExpanded, label, true);
            fieldPosition.yMin = fieldPosition.yMax + EditorGUIUtility.standardVerticalSpacing;

            if (!property.isExpanded)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope(1))
            {
                fieldPosition.yMax = fieldPosition.yMin + EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(fieldPosition, property.FindPropertyRelative("delay"));
                fieldPosition.yMin = fieldPosition.yMax + EditorGUIUtility.standardVerticalSpacing;

                fieldPosition.yMax = fieldPosition.yMin + EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(fieldPosition, property.FindPropertyRelative("printDebugMessages"));
                fieldPosition.yMin = fieldPosition.yMax + EditorGUIUtility.standardVerticalSpacing;

                fieldPosition.yMax = position.yMax;
                base.OnGUI(EditorGUI.IndentedRect(fieldPosition), property, new GUIContent("Event"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                height += EditorGUIUtility.standardVerticalSpacing;
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
                height += base.GetPropertyHeight(property, label);
            }
            return height;
        }
    }
}
