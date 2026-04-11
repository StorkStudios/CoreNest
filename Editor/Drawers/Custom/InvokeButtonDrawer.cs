using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class InvokeButtonDrawer
    {
        public MethodInfo Method => method;
        public string FoldoutGroupID => foldoutGroup?.Id;

        private MethodInfo method;
        private ParameterInfo[] parameters;
        private InvokeButtonAttribute invokeButton;

        private FoldoutGroupAttribute foldoutGroup;

        private bool foldout;

        private static (object value, Rect position) DrawField(Rect position, string label, Type type, object value)
        {
            if (type == typeof(int))
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                value = EditorGUI.IntField(position, label, (int)value);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            else if (type == typeof(float))
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                value = EditorGUI.FloatField(position, label, (float)value);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            else if (type == typeof(string))
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                value = EditorGUI.TextField(position, label, (string)value);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            else if (type == typeof(bool))
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                value = EditorGUI.Toggle(position, label, (bool)value);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            else if (type == typeof(Vector3))
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                value = EditorGUI.Vector3Field(position, label, (Vector3)value);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                value = EditorGUI.IntField(position, label, (int)value);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            else if (type.IsEnum)
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                value = EditorGUI.EnumPopup(position, label, (Enum)value);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            return (value, position);
        }

        public InvokeButtonDrawer(MethodInfo method)
        {
            this.method = method;
            parameters = method.GetParameters();
            invokeButton = GetCustomAttribute<InvokeButtonAttribute>();
        }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return method.GetCustomAttribute<T>();
        }

        public Rect Draw(Rect position, SerializedObject serializedObject)
        {
            bool pressed = false;

            if (parameters.Length <= 0)
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                pressed = GUI.Button(position, invokeButton.GetNameForMethod(method));
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;

                Rect labelRect = position;
                labelRect.xMax -= (position.width + EditorGUIUtility.standardVerticalSpacing) / 2;
                foldout = EditorGUI.Foldout(labelRect, foldout, $"Function: {invokeButton.GetNameForMethod(method)}", true);

                Rect buttonRect = position;
                buttonRect.xMin += (position.width + EditorGUIUtility.standardVerticalSpacing) / 2;
                pressed = GUI.Button(buttonRect, "Invoke");

                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (foldout)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        //TODO: methods with parameters
                        position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                        EditorGUI.LabelField(position, "Methods with parameters are not supported yet.");
                        position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            if (pressed)
            {
                foreach (UnityEngine.Object target in serializedObject.targetObjects)
                {
                    method.Invoke(target, null);
                }
            }

            return position;
        }

        public float GetHeight()
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (foldout)
            {
                //TODO: methods with parameters
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}