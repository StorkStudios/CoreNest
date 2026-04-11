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

                using (new EditorGUI.DisabledScope(true))
                {
                    Rect buttonRect = position;
                    buttonRect.xMin += (position.width + EditorGUIUtility.standardVerticalSpacing) / 2;
                    pressed = GUI.Button(buttonRect, "Invoke");
                }

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