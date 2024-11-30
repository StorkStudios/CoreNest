using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(SerializationArrayWrapper<string>))]
public class SerializationArrayWrapperDrawer : StatefulPropertyDrawer<SerializationArrayWrapperDrawer.State>
{
    public class State
    {
        public ReorderableList list;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        GetState(property).list.DoList(position);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        State state = GetState(property);
        if (state.list.index > state.list.count - 1)
        {
            state.list = CreateList(property);
        }
        return state.list.GetHeight();
    }

    protected override State CreateState(SerializedProperty property)
    {
        State state = new State();
        state.list = CreateList(property);
        return state;
    }

    private ReorderableList CreateList(SerializedProperty property)
    {
        SerializedProperty arrayProperty = property.FindPropertyRelative("Array");
        ReorderableList list = new ReorderableList(arrayProperty.serializedObject, arrayProperty, false, false, true, true);
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            rect.y += 2;
            EditorGUI.PropertyField(rect, arrayProperty.GetArrayElementAtIndex(index), true);
        };
        return list;
    }
}
