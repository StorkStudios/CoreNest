using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class ScrollView
    {
        private Vector2 scrollPosition;

        public void Begin()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        }

        public void End()
        {
            EditorGUILayout.EndScrollView();
        }
    }
}