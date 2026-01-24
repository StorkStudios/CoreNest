using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class SubAssetManagerWindow : EditorWindow
    {
        [SerializeField]
        private Object parentAsset;

        [MenuItem("Tools/Core'Nest/Sub-asset manager")]
        private static void Open()
        {
            GetWindow(typeof(SubAssetManagerWindow));
        }


        private static Object[] GetSubAssets(Object parentAsset)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(parentAsset));
        }

        private void OnGUI()
        {
            parentAsset = EditorGUILayout.ObjectField("Parent Asset", parentAsset, typeof(Object), false);
            if (parentAsset != null && !AssetDatabase.IsMainAsset(parentAsset))
            {
                parentAsset = null;
            }

            if (parentAsset == null)
            {
                EditorGUILayout.HelpBox("Select an asset to edit its sub-assets", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Sub-assets", GUIStyles.Bold);
            using (new EditorGUI.IndentLevelScope(1))
            {
                Object[] subAssets = GetSubAssets(parentAsset);
                if (subAssets.Length <= 0)
                {
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("No sub-assets in this asset", GUIStyles.Bold);
                    EditorGUILayout.Space(4);
                }
                foreach (Object subAsset in subAssets)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUIContent usedContent;
                        if (true)//todo: subAsset isUsed by main asset
                        {
                            usedContent = EditorGUIUtility.IconContent("BuildSettings.N3DS On");
                        }
                        else
                        {
                            usedContent = EditorGUIUtility.IconContent("BuildSettings.N3DS");
                        }
                        EditorGUILayout.LabelField(usedContent, EditorStyles.iconButton);

                        GUIContent thumbnail = new GUIContent();
                        thumbnail.image = AssetPreview.GetMiniThumbnail(subAsset);
                        if (GUILayout.Button(thumbnail, EditorStyles.iconButton))
                        {
                            EditorGUIUtility.PingObject(subAsset);
                        }

                        string newName = EditorGUILayout.DelayedTextField(subAsset.name);
                        if (subAsset.name != newName)
                        {
                            subAsset.name = newName;
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }
        }
    }
}