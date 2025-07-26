using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace StorkStudios.CoreNest
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
    public class SerializedDictionaryDrawer : StatefulPropertyDrawer<SerializedDictionaryDrawer.State>
    {
        public class State
        {
            public ReorderableList list;
        }

        private System.Type KeyType => fieldInfo.FieldType.GenericTypeArguments[0];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!KeyType.IsEnum)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("pairs"), label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            ReorderableList list = GetState(property).list;

            Rect labelRect = position;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            DrawHeaderCallback(list, property.displayName, labelRect);

            position.yMin += EditorGUIUtility.singleLineHeight;

            if (list.serializedProperty.isExpanded)
            {
                list.DoList(position);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!KeyType.IsEnum)
            {
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("pairs"), label);
            }

            State state = GetState(property);

            if (state.list.index > state.list.count - 1)
            {
                state.list = CreateList(property);
            }

            float result = EditorGUIUtility.singleLineHeight;

            if (state.list.serializedProperty.isExpanded)
            {
                result += state.list.GetHeight();
            }

            return result;
        }

        protected override State CreateState(SerializedProperty property)
        {
            State state = new State();

            state.list = CreateList(property);

            return state;
        }

        private ReorderableList CreateList(SerializedProperty property)
        {
            SerializedProperty pairsListProperty = property.FindPropertyRelative("pairs");

            ReorderableList list = new ReorderableList(pairsListProperty.serializedObject, pairsListProperty, false, false, true, true);

            list.drawElementCallback = (rect, index, isActive, isFocused) => DrawElementCallback(list, rect, index, isActive, isFocused);
            list.elementHeightCallback = (index) => ElementHeightCallback(list, index);
            list.onAddDropdownCallback = OnAddDropdownCallback;
            list.onCanAddCallback = CanAddCallback;

            return list;
        }

        private void DrawHeaderCallback(ReorderableList list, string displayName, Rect rect)
        {
            list.serializedProperty.isExpanded = EditorGUI.Foldout(rect, list.serializedProperty.isExpanded, displayName);
        }

        private void DrawElementCallback(ReorderableList list, Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        private float ElementHeightCallback(ReorderableList list, int index)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

            return EditorGUI.GetPropertyHeight(element);
        }

        private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            HashSet<string> existingValues = new HashSet<string>();
            foreach (SerializedProperty element in list.serializedProperty)
            {
                SerializedProperty keyProperty = element.FindPropertyRelative("Key").FindPropertyRelative("item");
                string valueName = keyProperty.enumNames[keyProperty.enumValueIndex];
                existingValues.Add(valueName);
            }

            GenericMenu menu = new GenericMenu();
            foreach (var enumValue in System.Enum.GetValues(KeyType))
            {
                if (existingValues.Contains(enumValue.ToString()))
                {
                    continue;
                }

                menu.AddItem(new GUIContent(enumValue.ToString()), false, (value) => AddClickHandler(list, value), enumValue);
            }

            menu.ShowAsContext();
        }

        private void AddClickHandler(ReorderableList list, object value)
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;

            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty key = element.FindPropertyRelative("Key").FindPropertyRelative("item");
            key.enumValueIndex = System.Array.IndexOf(System.Enum.GetValues(value.GetType()), value);

            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private bool CanAddCallback(ReorderableList list)
        {
            return list.serializedProperty.arraySize < System.Enum.GetValues(KeyType).Length;
        }
    }
}