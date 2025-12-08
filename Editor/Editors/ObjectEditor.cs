using UnityEngine;
using UnityEditor;

namespace StorkStudios.CoreNest
{
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