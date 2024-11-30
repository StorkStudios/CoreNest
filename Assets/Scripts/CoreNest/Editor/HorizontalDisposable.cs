using System;
using UnityEngine;

public class HorizontalDisposable : IDisposable
{
    public HorizontalDisposable()
    {
        GUILayout.BeginHorizontal();
    }

    public void Dispose()
    {
        GUILayout.EndHorizontal();
    }
}
