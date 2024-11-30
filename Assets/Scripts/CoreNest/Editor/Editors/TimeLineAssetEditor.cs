using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Timeline;
using System.Reflection;

[CustomEditor(typeof(TimelineAsset))]
[CanEditMultipleObjects]
public class TimelineAssetEditor : Editor
{
    private const double maxFixedTime = 2e6;

    private Editor defaultEditor = null;
    private TimelineAsset timelineAsset = null;

    private void OnEnable()
    {
        System.Type timelineAssetInspectorType = System.Type.GetType("UnityEditor.Timeline.TimelineAssetInspector, Unity.Timeline.Editor");
        if (timelineAssetInspectorType == null)
        {
            Debug.LogError("Couldn't find type of TimelineAssetInspector");
            return;
        }

        defaultEditor = CreateEditor(targets, timelineAssetInspectorType);
        timelineAsset = (TimelineAsset)target;
    }

    void OnDisable()
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

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("Extras", EditorStyles.boldLabel);


        TimelineAsset.DurationMode durationMode = (TimelineAsset.DurationMode)serializedObject.FindProperty("m_DurationMode").enumValueIndex;

        GUI.enabled = durationMode == TimelineAsset.DurationMode.FixedLength;
        if (GUILayout.Button("Set Infinite"))
        {
            timelineAsset.fixedDuration = maxFixedTime;
        }
        EditorGUILayout.LabelField("Note: Infinity lasts approximately 23 days", EditorStyles.miniLabel);
        GUI.enabled = true;
    }
}
