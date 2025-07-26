using System;
using UnityEngine;

namespace StorkStudios.CoreNest
{
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
}