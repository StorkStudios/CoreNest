using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Timeline;
using System.Linq;

namespace StorkStudios.CoreNest
{
    [CustomEditor(typeof(TimelineAsset))]
    [CanEditMultipleObjects]
    public class TimelineAssetEditor : BuiltInEditorExtensionBase
    {
        private const double maxFixedTime = 2e6;

        protected override string BuiltInEditorTypeName => "UnityEditor.Timeline.TimelineAssetInspector, Unity.Timeline.Editor";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("Extras", EditorStyles.boldLabel);

            IEnumerable<TimelineAsset> assetsViableForInfiniteTime = targets.Cast<TimelineAsset>().Where(e => e.durationMode == TimelineAsset.DurationMode.FixedLength);

            GUI.enabled = assetsViableForInfiniteTime.Count() > 0;
            if (GUILayout.Button("Set Infinite"))
            {
                foreach (TimelineAsset target in assetsViableForInfiniteTime)
                {
                    target.fixedDuration = maxFixedTime;
                }
            }
            EditorGUILayout.LabelField("Note: Infinity lasts approximately 23 days", EditorStyles.miniLabel);
            GUI.enabled = true;
        }
    }
}