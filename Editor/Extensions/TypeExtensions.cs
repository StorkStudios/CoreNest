using System;
using System.Collections.Generic;
using UnityEngine;

public static class TypeExtensions
{
    public static bool IsUnitySerializable(this Type type)
    {
        if (type.IsPrimitive || type == typeof(string))
            return true;

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            return true;

        if (type.IsEnum)
            return true;

        if (type.IsArray)
            return IsUnitySerializable(type.GetElementType());

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return IsUnitySerializable(type.GetGenericArguments()[0]);

        return type.IsDefined(typeof(SerializableAttribute), false);
    }
}
