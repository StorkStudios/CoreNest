using System;
using System.Reflection;
using UnityEditor;

public static class SerializedPropertyExtensions
{
    private const BindingFlags unitySerializableFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    /// <summary>
    /// Retrieves the reflection metadata for the field represented by the specified serialized property.
    /// </summary>
    /// <returns>A FieldInfo object representing the field associated with the property, or null if the field cannot be found.</returns>
    public static FieldInfo GetFieldInfo(this SerializedProperty property)
    {
        Type type = property.serializedObject.targetObject.GetType();
        while (type != null)
        {
            FieldInfo field = type.GetField(property.propertyPath, unitySerializableFlags);
            if (field != null)
            {
                return field;
            }
            type = type.BaseType;
        }
        return null;
    }

    /// <summary>
    /// Gets the type of elements in the array that contains this property. The array element type cannot be found by property path so <see cref="GetFieldInfo(SerializedProperty)"/> cannot be used.
    /// </summary>
    /// <returns>A Type object representing the type of element of array or null if the property isn't an array element or the array type isn't compatible.</returns>
    public static Type GetArrayElementPropertyType(this SerializedProperty property)
    {
        if (!property.propertyPath.Contains("Array.data["))
        {
            return null;
        }

        string arrayPropertyPath = property.propertyPath.Split(".Array.data[")[0];
        SerializedProperty arrayProperty = property.serializedObject.FindProperty(arrayPropertyPath);
        FieldInfo arrayFieldInfo = arrayProperty.GetFieldInfo();
        Type arrayType = arrayFieldInfo.FieldType;
        if (arrayType.IsArray)
        {
            return arrayType.GetElementType();
        }
        else if (arrayType.IsGenericType && arrayType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
        {
            return arrayType.GetGenericArguments()[0];
        }

        return null;
    }
}
