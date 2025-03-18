using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

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
        buttonsDrawer.Draw();
    }
}
