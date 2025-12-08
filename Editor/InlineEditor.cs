using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class InlineEditor
    {
        private readonly SerializedObject serializedObject;
        private readonly HashSet<string> drawnFoldouts = new HashSet<string>();

        private readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        public InlineEditor(SerializedObject objectToDraw)
        {
            serializedObject = objectToDraw;
        }

        public bool DrawInspector()
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(GetHeight()));
            return DrawInspector(rect);
        }

        // Copied from unity source code
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
            return result;
        }
    }
}