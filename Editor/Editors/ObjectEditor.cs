using UnityEngine;
using UnityEditor;

namespace StorkStudios.CoreNest
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class ObjectEditor : Editor
    {
        private InlineEditor inlineEditor;
        private ButtonsDrawer buttonsDrawer;

        private void OnEnable()
        {
            inlineEditor = new InlineEditor(serializedObject);
            buttonsDrawer = new ButtonsDrawer(target);
        }

        public override void OnInspectorGUI()
        {
            inlineEditor.DrawInspector();
            buttonsDrawer.Draw(targets);
        }
    }
}