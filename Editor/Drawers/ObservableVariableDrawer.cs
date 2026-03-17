using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(ObservableVariable<>))]
    public class ObservableVariableDrawer : StatefulPropertyDrawer<ObservableVariableDrawer.State>
    {
        public class State
        {
            public bool sendEvents = true;
            public bool delayedField = false;
            public bool runInEditMode = false;
        }

        private static void DelayedPropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.DelayedFloatField(position, label, property.floatValue);
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.DelayedIntField(position, label, property.intValue);
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = EditorGUI.DelayedTextField(position, label, property.stringValue);
                    break;
                default:
                    if (property.hasVisibleChildren)
                    {
                        position.height = EditorGUI.GetPropertyHeight(property, label, false);
                        EditorGUI.PropertyField(position, property, label, false);
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

                        if (property.isExpanded)
                        {
                            using (new EditorGUI.IndentLevelScope(1))
                            {
                                SerializedProperty iterator = property.Copy();
                                SerializedProperty endProperty = iterator.GetEndProperty();
                                while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, endProperty))
                                {
                                    GUIContent childLabel = EditorGUIUtility.TrTextContent(iterator.displayName);
                                    position.height = EditorGUI.GetPropertyHeight(iterator, childLabel, includeChildren);
                                    DelayedPropertyField(position, iterator, childLabel, includeChildren);
                                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                                }
                            }
                        }
                    }
                    else
                    {
                        EditorGUI.PropertyField(position, property, label, includeChildren);
                    }
                    break;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("current");
            if (valueProperty == null)
            {
                GUIContent content = new GUIContent(EditorGUIUtility.IconContent("console.warnicon"));
                content.text = label.text;
                EditorGUI.LabelField(position, content, new GUIContent($"Non serializable type used in {typeof(ObservableVariable<>).Name}"));
                return;
            }

            object current = valueProperty.boxedValue;

            State state = GetState(property);

            GUIStyle buttonStyle = EditorStyles.iconButton;

            EditorGUI.BeginChangeCheck();

            position.xMax -= buttonStyle.fixedWidth + EditorGUIUtility.standardVerticalSpacing;
            if (state.delayedField)
            {
                DelayedPropertyField(position, valueProperty, label, true);
            }
            else
            {
                EditorGUI.PropertyField(position, valueProperty, label, true);
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();

                if (state.sendEvents && (state.runInEditMode || EditorApplication.isPlaying))
                {
                    object newValue = valueProperty.boxedValue;
                    if (!current.Equals(newValue))
                    {
                        FieldInfo propertyInfo = property.GetFieldInfo();
                        MethodInfo valueChangedEvent = propertyInfo.FieldType.GetMethod("InvokeValueChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                        foreach (object target in property.serializedObject.targetObjects)
                        {
                            // property.boxedValue is a copy of the value, so we need to get the actual instance to invoke the event
                            object instance = propertyInfo.GetValue(target);
                            valueChangedEvent.Invoke(instance, new object[2] { current, newValue });
                        }
                    }
                }
            }

            position.xMin = position.xMax + EditorGUIUtility.standardVerticalSpacing;
            position.xMax += buttonStyle.fixedWidth;
            position.yMin += 1;
            position.yMax = position.yMin + buttonStyle.fixedHeight;

            string icon = "d_lightOff";
            if (state.sendEvents)
            {
                int dotIndex = (state.runInEditMode || EditorApplication.isPlaying) ? 1 : 4;
                dotIndex += state.delayedField ? 8 : 0;

                icon = $"sv_icon_dot{dotIndex}_pix16_gizmo";
            }

            if (GUI.Button(position, EditorGUIUtility.IconContent(icon), buttonStyle))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Don't send events"), !state.sendEvents, () => {
                    state.sendEvents = false;
                    state.runInEditMode = false;
                });
                menu.AddItem(new GUIContent("Send events in play mode"), state.sendEvents && !state.runInEditMode, () => {
                    state.sendEvents = true;
                    state.runInEditMode = false;
                });
                menu.AddItem(new GUIContent("Send events in edit and play mode"), state.runInEditMode, () => {
                    state.sendEvents = true;
                    state.runInEditMode = true;
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Use delayed fields"), state.delayedField, () => state.delayedField = !state.delayedField);

                menu.DropDown(position);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("current");
            if (valueProperty == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            return EditorGUI.GetPropertyHeight(valueProperty, label, true);
        }

        protected override State CreateState(SerializedProperty property)
        {
            return new State();
        }
    }
}