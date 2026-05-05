using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class InvokeButtonDrawer
    {
        public MethodInfo Method => method;
        public string FoldoutGroupID => foldoutGroup?.Id;

        private readonly MethodInfo method;
        private readonly IMethodParameters parameters;
        private readonly InlineEditor parametersEditor;
        private readonly InvokeButtonAttribute invokeButton;

        private readonly FoldoutGroupAttribute foldoutGroup;

        private bool isExpanded;

        public InvokeButtonDrawer(MethodInfo method)
        {
            this.method = method;
            Type parametersWrapperType = ScriptableObjectGenerator.GetScriptableObjectParametersWrapper(method);
            if (parametersWrapperType != null)
            {
                parameters = ScriptableObject.CreateInstance(parametersWrapperType) as IMethodParameters;
                parametersEditor = new InlineEditor(new SerializedObject(parameters as ScriptableObject)) { drawScriptField = false };
            }
            invokeButton = GetCustomAttribute<InvokeButtonAttribute>();
        }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return method.GetCustomAttribute<T>();
        }

        public Rect Draw(Rect position, SerializedObject serializedObject)
        {
            bool pressed = false;

            if (parameters == null)
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
                isExpanded = EditorGUI.Foldout(labelRect, isExpanded, $"Function: {invokeButton.GetNameForMethod(method)}", true);

                Rect buttonRect = position;
                buttonRect.xMin += (position.width + EditorGUIUtility.standardVerticalSpacing) / 2;
                pressed = GUI.Button(buttonRect, "Invoke");

                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        parametersEditor.DrawInspector(position, out position);
                    }
                }
            }

            if (pressed)
            {
                foreach (UnityEngine.Object target in serializedObject.targetObjects)
                {
                    method.Invoke(target, parameters?.GetValues());
                }
            }

            return position;
        }

        public float GetHeight()
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (isExpanded)
            {
                if (parameters != null)
                {
                    height += parametersEditor.GetHeight();
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return height;
        }
    }
}