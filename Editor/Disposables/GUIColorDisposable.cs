using System;
using UnityEngine;

namespace StorkStudios.CoreNest
{
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
}