using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheEditorStyleDisposable : IDisposable
{
    private readonly GUIStyle styleReference;
    private readonly GUIStyle cachedStyle;

    public CacheEditorStyleDisposable(GUIStyle styleToOverride)
    {
        styleReference = styleToOverride;
        cachedStyle = new GUIStyle(styleToOverride);
    }

    public void Dispose()
    {
        cachedStyle.CopyInto(styleReference);
    }
}
