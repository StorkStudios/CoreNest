using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIColorDisposable : IDisposable
{
    private Color color;

    public GUIColorDisposable()
    {
        color = GUI.color;
    }

    public void Dispose()
    {
        GUI.color = color;
    }
}
