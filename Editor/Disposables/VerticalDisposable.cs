using System;
using UnityEngine;

public class VerticalDisposable : IDisposable
{
    public VerticalDisposable(params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
    }

    public VerticalDisposable(GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(style, options);
    }

    public VerticalDisposable(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(content, style, options);
    }

    public VerticalDisposable(string text, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(text, style, options);
    }

    public VerticalDisposable(Texture image, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(image, style, options);
    }

    public void Dispose()
    {
        GUILayout.EndVertical();
    }
}
