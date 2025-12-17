using System;
using System.Reflection;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Specifies that a method should be displayed as an invokable button in the Unity Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InvokeButtonAttribute : PropertyAttribute
    {
        public string Name { get; private set; }

        public InvokeButtonAttribute(string name = null)
        {
            Name = name;
        }

        public string GetNameForMethod(MethodInfo methodInfo)
        {
            return Name ?? methodInfo.Name;
        }
    }
}