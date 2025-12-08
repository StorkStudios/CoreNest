using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

public static class SerializedPropertyExtensions
{
    private const BindingFlags unitySerializableFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static FieldInfo GetFieldInfo(this SerializedProperty property)
    {
        Type type = property.serializedObject.targetObject.GetType();
        return type.GetField(property.propertyPath, unitySerializableFlags);
    }
}
