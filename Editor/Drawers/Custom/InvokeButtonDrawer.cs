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

        private MethodInfo method;
        private IMethodParameters parameters;
        private Editor parametersEditor;
        private InvokeButtonAttribute invokeButton;

        private FoldoutGroupAttribute foldoutGroup;

        private bool foldout;

        public InvokeButtonDrawer(MethodInfo method)
        {
            this.method = method;
            Type parametersWrapperType = ScriptableObjectGenerator.GetScriptableObjectParametersWrapper(method);
            if (parametersWrapperType != null)
            {
                parameters = ScriptableObject.CreateInstance(parametersWrapperType) as IMethodParameters;
                parametersEditor = Editor.CreateEditor(parameters as ScriptableObject);
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
                foldout = EditorGUI.Foldout(labelRect, foldout, $"Function: {invokeButton.GetNameForMethod(method)}", true);

                Rect buttonRect = position;
                buttonRect.xMin += (position.width + EditorGUIUtility.standardVerticalSpacing) / 2;
                pressed = GUI.Button(buttonRect, "Invoke");

                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (foldout)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        parametersEditor.OnInspectorGUI();

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
                    method.Invoke(target, parameters?.GetValues());
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