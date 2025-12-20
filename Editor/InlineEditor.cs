using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Class implementing the drawing of an enhanced editor in the specified area.
    /// </summary>
    public class InlineEditor
    {
        private readonly SerializedObject serializedObject;
        private readonly HashSet<string> drawnFoldouts = new HashSet<string>();

        private readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
        private readonly Dictionary<MethodInfo, bool> methodFoldoutStates = new Dictionary<MethodInfo, bool>();

        private readonly List<MethodInfo> methods = new List<MethodInfo>();

        public InlineEditor(SerializedObject objectToDraw)
        {
            serializedObject = objectToDraw;

            // This can happen when drawing destroyed scripts
            if (serializedObject.targetObject == null)
            {
                return;
            }

            
            methods = GetInvokeButtonMethods();
        }

        private List<MethodInfo> GetInvokeButtonMethods()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            System.Type type = serializedObject.targetObject.GetType();
            List<MethodInfo> methods = new List<MethodInfo>();
            while (type != null)
            {
                foreach (MethodInfo method in type.GetMethods(bindingFlags).Where(e => e.GetCustomAttribute<InvokeButtonAttribute>() != null).Reverse())
                {
                    if (!methods.Contains(method))
                    {
                        methods.Add(method);
                    }
                }
                type = type.BaseType;
            }
            methods.Reverse();
            return methods;
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

            // m_Script field
            if (iterator.NextVisible(true))
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    float height = EditorGUI.GetPropertyHeight(iterator);
                    position.yMax = position.yMin + height;
                    EditorGUI.PropertyField(position, iterator, true);
                    position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            else
            {
                // this object has no properties
                return false;
            }

            if (serializedObject.targetObject == null)
            {
                return false;
            }

            while (iterator.NextVisible(false))
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
                    position.yMin = position.yMax;
                    if (height > 0)
                    {
                        position.yMin += EditorGUIUtility.standardVerticalSpacing;
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
                    ShowIfAttribute showIf = method.GetCustomAttribute<ShowIfAttribute>();
                    if (showIf != null)
                    {
                        position = ShowIfDrawer.Draw(position, method, showIf, serializedObject.targetObjects, position => DrawInvokeButton(position, method));
                    }
                    else
                    {
                        position = DrawInvokeButton(position, method);
                    }
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

            FoldoutGroupAttribute foldoutGroup = property.GetFieldInfo().GetCustomAttribute<FoldoutGroupAttribute>();
            string id = foldoutGroup.Id;
            string header = foldoutGroup.Header;
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
                    foldoutGroup = property.GetFieldInfo().GetCustomAttribute<FoldoutGroupAttribute>();
                    if (foldoutGroup == null || foldoutGroup.Id != id)
                    {
                        continue;
                    }

                    float height = EditorGUI.GetPropertyHeight(property);
                    position.yMax = position.yMin + height;
                    EditorGUI.PropertyField(position, property, true);
                    position.yMin = position.yMax;
                    if (height > 0)
                    {
                         position.yMin += EditorGUIUtility.standardVerticalSpacing;
                    }

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
                    ShowIfAttribute showIf = method.GetCustomAttribute<ShowIfAttribute>();
                    if (showIf != null)
                    {
                        position = ShowIfDrawer.Draw(position, method, showIf, serializedObject.targetObjects, position => DrawInvokeButton(position, method));
                    }
                    else
                    {
                        position = DrawInvokeButton(position, method);
                    }
                }
            }
            return position;
        }

        private Rect DrawInvokeButton(Rect position, MethodInfo method)
        {
            InvokeButtonAttribute invokeButton = method.GetCustomAttribute<InvokeButtonAttribute>();

            bool pressed = false;

            if (method.GetParameters().Length > 0)
            {
                if (!methodFoldoutStates.ContainsKey(method))
                {
                    methodFoldoutStates[method] = false;
                }

                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;

                Rect labelRect = position;
                labelRect.xMax -= (position.width + EditorGUIUtility.standardVerticalSpacing) / 2;
                methodFoldoutStates[method] = EditorGUI.Foldout(labelRect, methodFoldoutStates[method], $"Function: {invokeButton.GetNameForMethod(method)}", true);

                using (new EditorGUI.DisabledScope(true))
                {
                    Rect buttonRect = position;
                    buttonRect.xMin += (position.width + EditorGUIUtility.standardVerticalSpacing) / 2;
                    pressed = GUI.Button(buttonRect, "Invoke");
                }

                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (methodFoldoutStates[method])
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
            else
            {
                position.yMax = position.yMin + EditorGUIUtility.singleLineHeight;
                pressed = GUI.Button(position, invokeButton.GetNameForMethod(method));
                position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
                
            if (pressed)
            {
                foreach (Object target in serializedObject.targetObjects)
                {
                    method.Invoke(target, null);
                }
            }
            
            return position;
        }

        public float GetHeight()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            drawnFoldouts.Clear();
            float result = 0;

            // m_Script field
            if (iterator.NextVisible(true))
            {
                result += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                // this object has no properties
                return result;
            }

            if (serializedObject.targetObject == null)
            {
                return result;
            }

            while (iterator.NextVisible(false))
            {
                bool wouldDraw = true;

                FieldInfo field = iterator.GetFieldInfo();
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

                if (wouldDraw)
                {
                    float height = EditorGUI.GetPropertyHeight(iterator);
                    result += height;
                    if (height > 0)
                    {
                        result += EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }

            foreach (MethodInfo method in methods)
            {
                bool wouldDraw = true;
                float height = 0;

                FoldoutGroupAttribute foldout = method.GetCustomAttribute<FoldoutGroupAttribute>();

                if (foldout != null)
                {
                    if (!drawnFoldouts.Contains(foldout.Id))
                    {
                        drawnFoldouts.Add(foldout.Id);
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                    wouldDraw = foldoutStates.TryGetValue(foldout.Id, out bool value) && value;
                }

                if (wouldDraw)
                {
                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    if (methodFoldoutStates.TryGetValue(method, out bool visible) && visible)
                    {
                        //TODO: methods with parameters
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }

                ShowIfAttribute showIf = method.GetCustomAttribute<ShowIfAttribute>();

                if (showIf != null)
                {
                    height = ShowIfDrawer.GetHeight(height, showIf, serializedObject.targetObjects);
                }

                result += height;
            }

            return result;
        }
    }
}