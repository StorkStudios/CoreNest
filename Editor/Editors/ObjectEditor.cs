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

        private void OnEnable()
        {
            inlineEditor = new InlineEditor(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            inlineEditor.DrawInspector();
        }
    }
}