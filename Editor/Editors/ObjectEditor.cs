using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Object), true)]
public class ObjectEditor : Editor
{
    private ButtonsDrawer buttonsDrawer;

    private void OnEnable()
    {
        buttonsDrawer = new ButtonsDrawer(target);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        buttonsDrawer.Draw(targets);
    }
}
