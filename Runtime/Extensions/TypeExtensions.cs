using System;
using System.Linq;
using System.Reflection;

namespace StorkStudios.CoreNest
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Finds a member (field, property, or method) of the specified type based on a dot-separated path.
        /// </summary>
        public static MemberInfo FindMember(this Type type, string path, BindingFlags flags)
        {
            string[] parts = path.Split('.');
            Type currentType = type;
            foreach (string part in parts[0..^1])
            {
                switch (currentType.GetMember(part, flags).FirstOrDefault())
                {
                    case FieldInfo field:
                        currentType = field.FieldType;
                        break;
                    case PropertyInfo property:
                        currentType = property.PropertyType;
                        break;
                    default:
                        return null;
                }
            }
            return currentType.GetMember(parts[^1], flags).FirstOrDefault();
        }

        /// <summary>
        /// Finds a member (field, property, or method) of the specified type based on a dot-separated path and retrieves the parent object of the member.
        /// </summary>
        public static MemberInfo FindMember(this Type type, object instance, string path, BindingFlags flags, out object parent)
        {
            string[] parts = path.Split('.');
            Type currentType = type;
            parent = instance;
            foreach (string part in parts[0..^1])
            {
                switch (currentType.GetMember(part, flags).FirstOrDefault())
                {
                    case FieldInfo field:
                        parent = field.GetValue(parent);
                        currentType = field.FieldType;
                        break;
                    case PropertyInfo property:
                        parent = property.GetValue(parent);
                        currentType = property.PropertyType;
                        break;
                    default:
                        parent = null;
                        return null;
                }
            }
            return currentType.GetMember(parts[^1], flags).FirstOrDefault();
        }
    }
}