using UnityEngine;
using UnityEditor;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Provides a custom editor for Unity objects, enabling enhanced inspector functionality.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class ObjectEditor : Editor
    {
        private InlineEditor inlineEditor;

        public override void OnInspectorGUI()
        {
            inlineEditor ??= new InlineEditor(serializedObject);
            inlineEditor.DrawInspector();
        }
    }
}