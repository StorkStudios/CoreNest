using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public abstract class BuiltInEditorExtensionBase : Editor
{
    // "Namespace.Class.Name, Assembly.Name"
    protected abstract string BuiltInEditorTypeName { get; }

    private Editor defaultEditor = null;

    protected virtual void OnEnable()
    {
        System.Type defaultEditorType = System.Type.GetType(BuiltInEditorTypeName);
        if (defaultEditorType == null)
        {
            Debug.LogError($"Couldn't find editor of type '{BuiltInEditorTypeName}'");
            return;
        }

        defaultEditor = CreateEditor(targets, defaultEditorType);
    }

    protected virtual void OnDisable()
    {
        if (defaultEditor == null)
        {
            return;
        }

        MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        disableMethod?.Invoke(defaultEditor, null);
        DestroyImmediate(defaultEditor);
    }

    public override void OnInspectorGUI()
    {
        if (defaultEditor == null)
        {
            return;
        }

        defaultEditor.OnInspectorGUI();
        serializedObject.Update();
    }
}
