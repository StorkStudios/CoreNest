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
            foreach (string fieldName in path)
            {
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

        /// <summary>
        /// Gets the objects that contain the field represented by the specified serialized property.
        /// </summary>
        public static IEnumerable<object> GetParentObjects(this SerializedProperty property)
        {
            string[] path = property.propertyPath.Split('.');
            IEnumerable<object> objs = property.serializedObject.targetObjects;
            for (int i = 0; i < path.Length - 1; i++)
            {
                FieldInfo field = objs.First().GetType().GetField(path[i], unitySerializableFlags);
                if (field == null)
                {
                    return null;
                }
                objs = objs.Select(obj => field.GetValue(obj));
            }
            return objs;
        }
    }
}