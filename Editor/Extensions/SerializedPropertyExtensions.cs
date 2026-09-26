using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace StorkStudios.CoreNest
{
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

            string[] path = property.propertyPath.Split('.');

            FieldInfo field = null;
            for (int i = 0; i < path.Length; i++)
            {
                string fieldName = path[i];
                // Skip "Array.data[i]" parts of array elements' paths
                if (fieldName == "Array" && i + 1 < path.Length && path[i + 1].StartsWith("data["))
                {
                    // Array element itself isn't a field
                    if (i + 2 == path.Length)
                    {
                        return null;
                    }
                    type = type.GetCollectionElementType();
                    if (type == null)
                    {
                        return null;
                    }
                    i++;
                    continue;
                }

                Type searchType = type;
                while (searchType != null)
                {
                    field = searchType.GetField(fieldName, unitySerializableFlags);
                    if (field != null)
                    {
                        break;
                    }
                    searchType = searchType.BaseType;
                }

                if (field == null)
                {
                    return null;
                }

                type = field.FieldType;
            }
            return field;
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
            return arrayFieldInfo?.FieldType.GetCollectionElementType();
        }

        /// <summary>
        /// Gets the objects that contain the field represented by the specified serialized property.
        /// </summary>
        public static IEnumerable<object> GetParentObjects(this SerializedProperty property)
        {
            return property.serializedObject.targetObjects.Select(targetObject =>
            {
                targetObject.GetType().FindMember(targetObject, property.propertyPath, unitySerializableFlags, out object parent);
                return parent;
            });
        }
    }
}