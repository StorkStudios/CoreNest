using System;
using UnityEngine;

public class HorizontalDisposable : IDisposable
{
    public HorizontalDisposable(params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(options);
    }

    public HorizontalDisposable(GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(style, options);
    }

    public HorizontalDisposable(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(content, style, options);
    }

    public HorizontalDisposable(string text, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(text, style, options);
    }

    public HorizontalDisposable(Texture image, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(image, style, options);
    }

    public void Dispose()
    {
        GUILayout.EndHorizontal();
    }
}
