using System;
using System.Reflection;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Specifies a string value associated with a field. Used to provide a custom string representation for
    /// enumeration members.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StringValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }

        public StringValueAttribute(string value)
        {
            StringValue = value;
        }
    }

    public static class StringValueEnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());

            StringValueAttribute attribute = fieldInfo.GetCustomAttribute<StringValueAttribute>(false);

            return attribute?.StringValue;
        }
    }
}