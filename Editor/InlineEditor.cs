using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace StorkStudios.CoreNest
{
    public class InlineEditor
    {
        private readonly SerializedObject serializedObject;
        private readonly HashSet<string> drawnFoldouts = new HashSet<string>();

        private readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        private readonly List<MethodInfo> methods;

        public InlineEditor(SerializedObject objectToDraw)
        {
            serializedObject = objectToDraw;

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            methods = serializedObject.targetObject.GetType().GetMethods(bindingFlags).Where(e => e.GetCustomAttribute<InvokeButtonAttribute>() != null).ToList();
        }

        public bool DrawInspector()
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(GetHeight()));
            return DrawInspector(rect);
        }

        public bool DrawInspector(Rect position)
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            drawnFoldouts.Clear();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.propertyPath == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        float height = EditorGUI.GetPropertyHeight(iterator);
                        position.yMax = position.yMin + height;
                        EditorGUI.PropertyField(position, iterator, true);
                        position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                    }
                    enterChildren = false;
                }
                else
                {
                    FieldInfo field = iterator.GetFieldInfo();
                    FoldoutGroupAttribute foldout = field.GetCustomAttribute<FoldoutGroupAttribute>();

                    if (foldout != null)
                    {
                        if (!drawnFoldouts.Contains(foldout.Id))
                        {
                            drawnFoldouts.Add(foldout.Id);
                            position = DrawFoldoutGroup(iterator, position);
                        }
                    }
                    else
                    {
                        float height = EditorGUI.GetPropertyHeight(iterator);
                        position.yMax = position.yMin + height;
                        EditorGUI.PropertyField(position, iterator, true);
                        position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            foreach (MethodInfo method in methods)
            {
                FoldoutGroupAttribute foldout = method.GetCustomAttribute<FoldoutGroupAttribute>();

                if (foldout != null)
                {
                    if (!drawnFoldouts.Contains(foldout.Id))
                    {
                        drawnFoldouts.Add(foldout.Id);
                        position = DrawFoldoutGroupButtons(position, foldout.Id, true);
                    }
                }
                else
                {
                    float height = EditorGUIUtility.singleLineHeight;

                    //TODO: support parameters
                    if (method.GetParameters().Length > 0)
                    {
                        position.yMax = position.yMin + height;
                        EditorGUI.LabelField(position, $"'{method.Name}' has parameters. It isn't compatible with Invoke Button");
                        position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                        continue;
                    }

                    InvokeButtonAttribute invokeButton = method.GetCustomAttribute<InvokeButtonAttribute>();

                    position.yMax = position.yMin + height;
                    if (GUI.Button(position, invokeButton.GetNameForMethod(method)))
                    {
                        foreach (Object target in serializedObject.targetObjects)
                        {
                            method.Invoke(target, null);
                        }
                    }
                    position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            bool changed = EditorGUI.EndChangeCheck();
            if (changed)
            {
                serializedObject.ApplyModifiedProperties();
            }

            return changed;
        }

        private Rect DrawFoldoutGroup(SerializedProperty property, Rect position)
        {
            property = property.Copy();

            FoldoutGroupAttribute foldoutGoup = property.GetFieldInfo().GetCustomAttribute<FoldoutGroupAttribute>();
            string id = foldoutGoup.Id;
            string header = foldoutGoup.Header;
            if (!foldoutStates.ContainsKey(id))
            {
                foldoutStates[id] = false;
            }

            position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
            foldoutStates[id] = EditorGUI.Foldout(position, foldoutStates[id], header, true);
            position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;

            if (!foldoutStates[id])
            {
                return position;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                do
                {
                    foldoutGoup = property.GetFieldInfo().GetCustomAttribute<FoldoutGroupAttribute>();
                    if (foldoutGoup == null || foldoutGoup.Id != id)
                    {
                        continue;
                    }

                    position.yMax = position.yMin + EditorGUI.GetPropertyHeight(property);
                    EditorGUI.PropertyField(position, property, true);
                    position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;

                } while (property.NextVisible(false));
            }
            return DrawFoldoutGroupButtons(position, id, false);
        }

        private Rect DrawFoldoutGroupButtons(Rect position, string id, bool drawHeader)
        {
            IEnumerable<MethodInfo> methodsWithFoldout = methods.Where(e => e.GetCustomAttribute<FoldoutGroupAttribute>() != null);

            if (!foldoutStates.ContainsKey(id))
            {
                foldoutStates[id] = false;
            }

            if (drawHeader)
            {
                string header = methodsWithFoldout.First(e => e.GetCustomAttribute<FoldoutGroupAttribute>().Id == id).GetCustomAttribute<FoldoutGroupAttribute>().Header;

                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                foldoutStates[id] = EditorGUI.Foldout(position, foldoutStates[id], header, true);
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!foldoutStates[id])
            {
                return position;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                foreach (MethodInfo method in methodsWithFoldout.Where(e => e.GetCustomAttribute<FoldoutGroupAttribute>().Id == id))
                {
                    float height = EditorGUIUtility.singleLineHeight;

                    //TODO: support parameters
                    if (method.GetParameters().Length > 0)
                    {
                        position.yMax = position.yMin + height;
                        EditorGUI.LabelField(position, $"'{method.Name}' has parameters. It isn't compatible with Invoke Button");
                        position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                        continue;
                    }

                    InvokeButtonAttribute invokeButton = method.GetCustomAttribute<InvokeButtonAttribute>();

                    position.yMax = position.yMin + height;
                    if (GUI.Button(position, invokeButton.GetNameForMethod(method)))
                    {
                        foreach (Object target in serializedObject.targetObjects)
                        {
                            method.Invoke(target, null);
                        }
                    }
                    position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return position;
        }

        public float GetHeight()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            drawnFoldouts.Clear();
            bool enterChildren = true;
            float result = 0;
            while (iterator.NextVisible(enterChildren))
            {
                bool wouldDraw = true;

                FieldInfo field = iterator.GetFieldInfo();

                if (field != null)
                {
                    FoldoutGroupAttribute foldout = field.GetCustomAttribute<FoldoutGroupAttribute>();
                    if (foldout != null)
                    {
                        if (!drawnFoldouts.Contains(foldout.Id))
                        {
                            drawnFoldouts.Add(foldout.Id);
                            result += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        }
                        wouldDraw = foldoutStates.TryGetValue(foldout.Id, out bool value) && value;
                    }
                }

                if (wouldDraw)
                {
                    result += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                }

                enterChildren = false;
            }

            foreach (MethodInfo method in methods)
            {
                bool wouldDraw = true;

                FoldoutGroupAttribute foldout = method.GetCustomAttribute<FoldoutGroupAttribute>();

                if (foldout != null)
                {
                    if (!drawnFoldouts.Contains(foldout.Id))
                    {
                        drawnFoldouts.Add(foldout.Id);
                        result += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                    wouldDraw = foldoutStates.TryGetValue(foldout.Id, out bool value) && value;
                }

                float height = EditorGUIUtility.singleLineHeight;
                if (method.GetParameters().Length > 0)
                {
                    //TODO: support parameters
                }

                if (wouldDraw)
                {
                    result += height + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return result;
        }
    }
}