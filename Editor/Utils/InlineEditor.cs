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

        private readonly List<InvokeButtonDrawer> invokeButtonDrawers = new List<InvokeButtonDrawer>();

        public InlineEditor(SerializedObject objectToDraw)
        {
            serializedObject = objectToDraw;

            // This can happen when drawing destroyed scripts
            if (serializedObject.targetObject == null)
            {
                return;
            }


            invokeButtonDrawers = GetInvokeButtonDrawers();
        }

        private List<InvokeButtonDrawer> GetInvokeButtonDrawers()
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

            return methods.Select(method => new InvokeButtonDrawer(method)).ToList();
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
                enterChildren = false;

                if (iterator.propertyPath == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        float height = EditorGUI.GetPropertyHeight(iterator);
                        position.yMax = position.yMin + height;
                        EditorGUI.PropertyField(position, iterator, true);
                        position.yMin = position.yMax + EditorGUIUtility.standardVerticalSpacing;
                    }
                    continue;
                }

                FieldInfo field = iterator.GetFieldInfo();
                FoldoutGroupAttribute foldout = field?.GetCustomAttribute<FoldoutGroupAttribute>();

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

            foreach (InvokeButtonDrawer drawer in invokeButtonDrawers)
            {
                FoldoutGroupAttribute foldout = drawer.GetCustomAttribute<FoldoutGroupAttribute>();

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
                    ShowIfAttribute showIf = drawer.GetCustomAttribute<ShowIfAttribute>();
                    if (showIf != null)
                    {
                        position = ShowIfDrawer.Draw(position, drawer.Method, showIf, serializedObject.targetObjects, position => drawer.Draw(position, serializedObject));
                    }
                    else
                    {
                        position = drawer.Draw(position, serializedObject);
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
            IEnumerable<InvokeButtonDrawer> drawersWithFoldout = invokeButtonDrawers.Where(e => {
                FoldoutGroupAttribute f = e.GetCustomAttribute<FoldoutGroupAttribute>();
                return f != null && f.Id == id;
                });

            if (!foldoutStates.ContainsKey(id))
            {
                foldoutStates[id] = false;
            }

            if (drawHeader)
            {
                string header = drawersWithFoldout.First().GetCustomAttribute<FoldoutGroupAttribute>().Header;

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
                foreach (InvokeButtonDrawer drawer in drawersWithFoldout)
                {
                    ShowIfAttribute showIf = drawer.GetCustomAttribute<ShowIfAttribute>();
                    if (showIf != null)
                    {
                        position = ShowIfDrawer.Draw(position, drawer.Method, showIf, serializedObject.targetObjects, position => drawer.Draw(position, serializedObject));
                    }
                    else
                    {
                        position = drawer.Draw(position, serializedObject);
                    }
                }
            }
            return position;
        }

        public float GetHeight()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            drawnFoldouts.Clear();
            float result = 0;

            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyPath == "m_Script")
                {
                    result += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                    continue;
                }

                bool wouldDraw = true;

                FieldInfo field = iterator.GetFieldInfo();
                FoldoutGroupAttribute foldout = field?.GetCustomAttribute<FoldoutGroupAttribute>();

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

            foreach (InvokeButtonDrawer drawer in invokeButtonDrawers)
            {
                bool wouldDraw = true;
                float height = 0;

                FoldoutGroupAttribute foldout = drawer.GetCustomAttribute<FoldoutGroupAttribute>();

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
                    height += drawer.GetHeight();
                }

                ShowIfAttribute showIf = drawer.GetCustomAttribute<ShowIfAttribute>();

                if (showIf != null)
                {
                    height = ShowIfDrawer.GetHeight(height, showIf, serializedObject.targetObjects);
                }

                result += height;
            }

            result -= EditorGUIUtility.standardVerticalSpacing; // remove last spacing
            return result;
        }
    }
}