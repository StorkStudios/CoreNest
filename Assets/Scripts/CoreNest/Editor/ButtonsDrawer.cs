using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ButtonsDrawer
{
    private readonly List<(MethodInfo method, InvokeButtonAttribute attribute)> buttons = new List<(MethodInfo method, InvokeButtonAttribute attribute)>();

    public ButtonsDrawer(Object target)
    {
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        MethodInfo[] methods = target.GetType().GetMethods(bindingFlags);

        foreach (MethodInfo method in methods)
        {
            InvokeButtonAttribute attribute = method.GetCustomAttribute<InvokeButtonAttribute>();

            if (attribute == null)
            {
                continue;
            }

            buttons.Add((method, attribute));
        }
    }

    public void Draw(IEnumerable<Object> targets)
    {
        foreach ((MethodInfo method, InvokeButtonAttribute attribute) in buttons)
        {
            if (GUILayout.Button(attribute.GetNameForMethod(method)))
            {
                foreach (Object target in targets)
                {
                    method.Invoke(target, null);
                }
            }
        }
    }
}
