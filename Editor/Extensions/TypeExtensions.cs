using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TypeExtensions
{
    private static readonly HashSet<Type> unityBuiltInTypes = new HashSet<Type>()
    {
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector4),
        typeof(Vector2Int),
        typeof(Vector3Int),
        typeof(Color),
        typeof(Color32),
        typeof(Rect),
        typeof(RectInt),
        typeof(Bounds),
        typeof(BoundsInt),
        typeof(Quaternion),
        typeof(AnimationCurve),
        typeof(LayerMask),
        typeof(Gradient),
    };

    public static bool IsUnitySerializable(this Type type)
    {
        if (unityBuiltInTypes.Contains(type))
        {
            return true;
        }

        if (type.IsPrimitive || type == typeof(string))
        {
            return true;
        }

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            return true;
        }

        if (type.IsEnum)
        {
            return true;
        }

        if (type.IsArray)
        {
            return IsUnitySerializable(type.GetElementType());
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            return IsUnitySerializable(type.GetGenericArguments()[0]);
        }

        return type.IsDefined(typeof(SerializableAttribute), false);
    }
}
