using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorkStudios.CoreNest
{
    public static class TypeExtensions
    {
        private const string arrayPathPart = "Array";
        private const string arrayDataPathPrefix = "data[";

        /// <summary>
        /// Finds a member (field, property, or method) of the specified type based on a dot-separated path.
        /// Supports Unity serialized property paths with array elements (e.g. list.Array.data[0].field).
        /// </summary>
        public static MemberInfo FindMember(this Type type, string path, BindingFlags flags)
        {
            return type.FindMember(null, path, flags, out _);
        }

        /// <summary>
        /// Finds a member (field, property, or method) of the specified type based on a dot-separated path and retrieves the parent object of the member.
        /// Supports Unity serialized property paths with array elements (e.g. list.Array.data[0].field).
        /// If the path points to an array element, the array member and the object containing it are returned.
        /// </summary>
        public static MemberInfo FindMember(this Type type, object instance, string path, BindingFlags flags, out object parent)
        {
            List<string> parts = SplitPath(path);
            // An array element has no MemberInfo, use the array member instead
            while (parts.Count > 0 && TryParseIndex(parts[^1], out _))
            {
                parts.RemoveAt(parts.Count - 1);
            }

            Type currentType = type;
            parent = instance;
            foreach (string part in parts.Take(parts.Count - 1))
            {
                if (TryParseIndex(part, out int index))
                {
                    currentType = currentType.GetCollectionElementType();
                    parent = parent is IList list && index < list.Count ? list[index] : null;
                    if (currentType == null)
                    {
                        parent = null;
                        return null;
                    }
                    continue;
                }

                switch (currentType.GetMember(part, flags).FirstOrDefault())
                {
                    case FieldInfo field:
                        parent = parent == null ? null : field.GetValue(parent);
                        currentType = field.FieldType;
                        break;
                    case PropertyInfo property:
                        parent = parent == null ? null : property.GetValue(parent);
                        currentType = property.PropertyType;
                        break;
                    default:
                        parent = null;
                        return null;
                }
            }
            return parts.Count == 0 ? null : currentType.GetMember(parts[^1], flags).FirstOrDefault();
        }

        /// <summary>
        /// Gets the element type of an array or a list.
        /// </summary>
        /// <returns>Type of the collection elements or null if the type isn't an array or a list.</returns>
        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return type.GetGenericArguments()[0];
            }
            return null;
        }

        /// <summary>
        /// Splits the path by dots and merges Unity's "Array.data[i]" parts into a single "[i]" part.
        /// </summary>
        private static List<string> SplitPath(string path)
        {
            string[] rawParts = path.Split('.');
            List<string> parts = new List<string>(rawParts.Length);
            for (int i = 0; i < rawParts.Length; i++)
            {
                if (rawParts[i] == arrayPathPart &&
                    i + 1 < rawParts.Length &&
                    rawParts[i + 1].StartsWith(arrayDataPathPrefix))
                {
                    parts.Add("[" + rawParts[i + 1][arrayDataPathPrefix.Length..^1] + "]");
                    i++;
                    continue;
                }
                parts.Add(rawParts[i]);
            }
            return parts;
        }

        private static bool TryParseIndex(string part, out int index)
        {
            index = -1;
            return part.StartsWith("[") && part.EndsWith("]") && int.TryParse(part[1..^1], out index);
        }
    }
}
