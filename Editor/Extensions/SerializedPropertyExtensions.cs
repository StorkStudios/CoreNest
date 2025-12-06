using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SerializedPropertyExtensions
{
    public static FieldInfo GetFieldInfo(this SerializedProperty property)
    {
        Type type = property.serializedObject.targetObject.GetType();
        return type.GetField(property.propertyPath);
    }
}
