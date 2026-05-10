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

        private bool isExpanded = false;

        public InvokeButtonDrawer(MethodInfo method)
        {
            this.method = method;

            parameters = ScriptableObjectTypeGenerator.CreateParametersWrapperInstance(method) as IMethodParameters;
            if (parameters != null)
            {
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

            if (parameters?.Count <= 0)
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
                using (new EditorGUI.DisabledScope(parameters == null))
                {
                    pressed = GUI.Button(buttonRect, "Invoke");
                }

                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        if (parameters != null)
                        {
                            position = parametersEditor.DrawInspector(position);
                        }
                        else
                        {
                            position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                            EditorGUI.LabelField(position, "Method has parameters but some aren't serializable.");
                            position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                        }
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