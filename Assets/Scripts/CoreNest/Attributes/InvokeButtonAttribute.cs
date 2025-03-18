using System;
using System.Reflection;
using UnityEngine;

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
