using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class StatefulPropertyDrawer<StateType> : PropertyDrawer where StateType : class
{
    private Dictionary<string, StateType> states;

    protected abstract StateType CreateState(SerializedProperty property);

    protected StateType GetState(SerializedProperty property)
    {
        states ??= new Dictionary<string, StateType>();

        string key = property.propertyPath;
        if (!states.ContainsKey(key))
        {
            states.Add(key, CreateState(property));
        }

        return states[key];
    }
}
