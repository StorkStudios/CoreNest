using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIEnabledDisposable : IDisposable
{
    private readonly bool wasEnabled;

    public GUIEnabledDisposable(bool enabled)
    {
        wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
    }

    public void Dispose()
    {
        GUI.enabled = wasEnabled;
    }
}
