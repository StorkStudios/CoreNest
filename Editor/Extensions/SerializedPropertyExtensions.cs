using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

public static class SerializedPropertyExtensions
{
    private const BindingFlags unitySerializableFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Retrieves the reflection metadata for the field represented by the specified serialized property.
    /// </summary>
    /// <returns>A FieldInfo object representing the field associated with the property, or null if the field cannot be found.</returns>
    public static FieldInfo GetFieldInfo(this SerializedProperty property)
    {
        Type type = property.serializedObject.targetObject.GetType();
        return type.GetField(property.propertyPath, unitySerializableFlags);
    }
}
