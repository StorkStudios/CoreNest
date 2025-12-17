using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        ShowIfAttribute showIfAttribute = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            showIfAttribute ??= (ShowIfAttribute)attribute;

            UObject[] targets = property.serializedObject.targetObjects;

            Draw(position, property.GetFieldInfo(), showIfAttribute, targets,
                pos =>
                {
                    EditorGUI.PropertyField(pos, property, label);
                    return pos;
                });
        }

        public static Rect Draw(Rect position, MemberInfo member, ShowIfAttribute attribute, IEnumerable<UObject> targets, Func<Rect, Rect> drawFunction)
        {
            bool? visible = attribute.ShouldShow(targets.First());
            if (!visible.HasValue)
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;

                string message = "Show if can only reference bool field, property or parameterless method.";
                GUIContent content = EditorGUIUtility.IconContent("console.warnicon");
                content.text = message;
                content.tooltip = $"{member.DeclaringType.Name}.{member.Name}";
                EditorGUI.LabelField(position, content);

                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                return position;
            }
            if (targets.All(e => attribute.ShouldShow(e).Value == visible.Value))
            {
                if (visible.Value)
                {
                    return drawFunction.Invoke(position);
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                using (new EditorGUI.MixedValueScope(true))
                {
                    return drawFunction.Invoke(position);
                }
            }
            return position;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            showIfAttribute ??= (ShowIfAttribute)attribute;

            UObject[] targets = property.serializedObject.targetObjects;

            return GetHeight(EditorGUI.GetPropertyHeight(property), showIfAttribute, targets);
        }

        public static float GetHeight(float normalHeight, ShowIfAttribute attribute, IEnumerable<UObject> targets)
        {
            bool? visible = attribute.ShouldShow(targets.First());
            if (!visible.HasValue)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            if (targets.All(e => attribute.ShouldShow(e).Value == visible.Value))
            {
                return visible.Value ? normalHeight : 0;
            }
            return normalHeight;
        }
    }
}