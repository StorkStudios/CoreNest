using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; private set; }

        public ShowIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
        }

        public bool? ShouldShow(object target)
        {
            if (target == null)
            {
                return null;
            }

            Type type = target.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MemberInfo member = type.GetMember(ConditionName, bindingFlags).FirstOrDefault();
            switch (member)
            {
                case FieldInfo field:
                    return field.GetValue(target) as bool?;
                case PropertyInfo property:
                    return property.GetValue(target) as bool?;
                case MethodInfo method:
                    if (method.GetParameters().Length > 0)
                    {
                        return null;
                    }
                    return method.Invoke(target, null) as bool?;
            }
            return null;
        }
    }
}
