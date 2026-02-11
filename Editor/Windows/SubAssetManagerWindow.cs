using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public partial class SubAssetManagerWindow : EditorWindow
    {
        private static readonly GUIContent openManagerWindow = new GUIContent("Open sub-asset manager");
        private static readonly GUIContent moveSubAssetOption = new GUIContent("Move into sub-asset");
        private static readonly GUIContent copySubAssetOption = new GUIContent("Create sub-asset copy");

        [SerializeField]
        private Object parentAsset;

        [SerializeField]
        private Object customAssetToAdd;

        public static void DrawSubAssetMenu(Rect position, GUIStyle buttonStyle, Object asset, string parentAssetPath, bool displaySubAssetManagerOption, System.Action<Object> onCopyCreated)
        {
            if (GUI.Button(position, EditorGUIUtility.IconContent("d__Menu@2x"), buttonStyle))
            {
                GenericMenu menu = new GenericMenu();

                if (displaySubAssetManagerOption)
                {
                    menu.AddItem(openManagerWindow, false, () => Open(AssetDatabase.LoadMainAssetAtPath(parentAssetPath)));
                }

                bool canInstanceBeSubAsset = SubAssetUtils.CanInstanceBeSubAsset(asset, parentAssetPath);

                if (asset != null && canInstanceBeSubAsset)
                {
                    menu.AddItem(moveSubAssetOption, false, () =>
                    {
                        SubAssetUtils.MoveIntoSubAsset(asset, parentAssetPath);
                        AssetDatabase.SaveAssets();
                    });
                }
                else
                {
                    menu.AddDisabledItem(moveSubAssetOption);
                }

                if (asset != null && !SubAssetUtils.IsSubAssetOf(asset, parentAssetPath))
                {
                    menu.AddItem(copySubAssetOption, false, () =>
                    {
                        Object copy = SubAssetUtils.MakeSubAssetCopy(asset, parentAssetPath);
                        onCopyCreated?.Invoke(copy);
                        AssetDatabase.SaveAssets();
                    });
                }
                else
                {
                    menu.AddDisabledItem(copySubAssetOption);
                }
                menu.DropDown(position);
            }
        }

        [MenuItem("Tools/Core'Nest/Sub-asset manager")]
        private static void Open()
        {
            GetWindow(typeof(SubAssetManagerWindow));
        }

        private static void Open(Object asset)
        {
            SubAssetManagerWindow window = GetWindow<SubAssetManagerWindow>();
            window.parentAsset = asset;
        }

        private static string GetAssetPropertyPaths(Object asset, List<SerializedProperty> properties)
        {
            if (properties.Count == 0)
            {
                return "";
            }

            bool first = true;
            StringBuilder sb = new StringBuilder();
            foreach (SerializedProperty property in properties.Where(e => e.serializedObject.targetObject != null))
            {
                if (!first)
                {
                    sb.AppendLine();
                }

                sb.Append(property.serializedObject.targetObject.name);
                sb.Append("/");
                sb.Append(property.propertyPath);
                first = false;
            }
            return sb.ToString();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(2);

            parentAsset = EditorGUILayout.ObjectField("Parent Asset", parentAsset, typeof(Object), false);
            if (parentAsset != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(parentAsset)))
            {
                parentAsset = null;
            }

            if (parentAsset == null)
            {
                EditorGUILayout.HelpBox("Select an asset to edit its sub-assets", MessageType.Info);
                return;
            }

            AssetInfo assetInfo = new AssetInfo(parentAsset);
            parentAsset = assetInfo.mainAsset;

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Sub-assets", GUIStyles.Bold);
            using (new EditorGUI.IndentLevelScope(1))
            {
                IEnumerable<Object> subAssets = assetInfo.subAssets.Keys;
                foreach (Object subAsset in subAssets)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        float iconWidth = EditorStyles.iconButton.fixedWidth;
                        Rect controlRect = EditorGUILayout.GetControlRect();
                        Rect rect = EditorGUI.IndentedRect(controlRect);

                        bool isSubAssetUsed = assetInfo.subAssets[subAsset].Count > 0;
                        bool isSubAssetUsedByMain = assetInfo.IsAssetUsedByMainAsset(subAsset);

                        string usedIcon = (isSubAssetUsed, isSubAssetUsedByMain) switch
                        {
                            (true, true) => "greenLight",
                            (true, false) => "orangeLight",
                            _ => "lightOff"
                        };
                        rect.xMax = rect.xMin + iconWidth;
                        GUIContent usedContent = new GUIContent(EditorGUIUtility.IconContent(usedIcon))
                        {
                            tooltip = isSubAssetUsed ? GetAssetPropertyPaths(subAsset, assetInfo.subAssets[subAsset]) : "This sub asset is not used in any of the assets in file"
                        };
                        GUI.Label(rect, usedContent, GUIStyles.IconLabel);
                        GUI.Label(rect, EditorGUIUtility.IconContent("lightRim"), GUIStyles.IconLabel);

                        rect.x += rect.width + EditorGUIUtility.standardVerticalSpacing;
                        GUIContent thumbnail = new GUIContent { image = AssetPreview.GetMiniThumbnail(subAsset) };
                        if (GUI.Button(rect, thumbnail, EditorStyles.iconButton))
                        {
                            EditorGUIUtility.PingObject(subAsset);
                        }

                        rect.xMin = controlRect.xMin + (iconWidth + EditorGUIUtility.standardVerticalSpacing) * 2;
                        rect.xMax = controlRect.xMax - (iconWidth + EditorGUIUtility.standardVerticalSpacing) * 1;
                        string newName = EditorGUI.DelayedTextField(rect, subAsset.name);
                        if (subAsset.name != newName)
                        {
                            subAsset.name = newName;
                            AssetDatabase.SaveAssets();
                        }

                        rect.xMin = rect.xMax + EditorGUIUtility.standardVerticalSpacing;
                        rect.xMax = rect.xMin + iconWidth;
                        GUIContent deleteContent = new GUIContent(EditorGUIUtility.IconContent("P4_DeletedLocal@2x"))
                        {
                            tooltip = "Delete this sub-asset"
                        };
                        if (GUI.Button(rect, deleteContent, EditorStyles.iconButton) &&
                            EditorUtility.DisplayDialog("SubAsset: Destroy sub-asset", $"Are you sure you want to destroy sub-asset \"{subAsset.name}\"?. Consider making a copy.", "Destroy", "Cancel"))
                        {
                            DestroyImmediate(subAsset, true);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
                if (subAssets.Count() <= 0)
                {
                    EditorGUILayout.HelpBox("No sub-assets in this asset", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.LabelField("(green - used by main asset, yellow - used by unused sub-asset, none - unused)");
                }
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Assets used by the parent asset", GUIStyles.Bold);
            using (new EditorGUI.IndentLevelScope(1))
            {
                IEnumerable<Object> dependencyAssets = assetInfo.dependencyAssets.Keys.Where(e => !assetInfo.subAssets.ContainsKey(e));
                if (dependencyAssets.Count() <= 0)
                {
                    EditorGUILayout.HelpBox("No sub-assetable external assets are used by this asset", MessageType.Info);
                }
                foreach (Object asset in dependencyAssets)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        Rect rect =  EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
                        GUIContent content = new GUIContent(EditorGUIUtility.ObjectContent(asset, asset.GetType()));
                        content.tooltip = GetAssetPropertyPaths(asset, assetInfo.dependencyAssets[asset]);
                        if (GUI.Button(rect, content, EditorStyles.objectField))
                        {
                            EditorGUIUtility.PingObject(asset);
                        }

                        GUIStyle iconStyle = new GUIStyle(EditorStyles.iconButton);
                        iconStyle.margin.top += 2;
                        Rect buttonRect = GUILayoutUtility.GetRect(EditorGUIUtility.IconContent("d__Menu@2x"), iconStyle);
                        DrawSubAssetMenu(buttonRect, iconStyle, asset, AssetDatabase.GetAssetPath(assetInfo.mainAsset), false, copy =>
                        {
                            foreach (SerializedProperty property in assetInfo.dependencyAssets[asset])
                            {
                                property.objectReferenceValue = copy;
                                property.serializedObject.ApplyModifiedProperties();
                            }
                        });
                    }
                }
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Add another asset", GUIStyles.Bold);
            bool isDisabled = false;
            using (new EditorGUI.IndentLevelScope(1))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    customAssetToAdd = EditorGUILayout.ObjectField(customAssetToAdd, typeof(Object), false);

                    GUIStyle iconStyle = new GUIStyle(EditorStyles.iconButton);
                    iconStyle.margin.top += 2;
                    Rect buttonRect = GUILayoutUtility.GetRect(EditorGUIUtility.IconContent("d__Menu@2x"), iconStyle);
                    isDisabled = customAssetToAdd == null || !SubAssetUtils.CanInstanceBeSubAsset(customAssetToAdd, AssetDatabase.GetAssetPath(assetInfo.mainAsset));
                    using (new EditorGUI.DisabledScope(isDisabled))
                    {
                        DrawSubAssetMenu(buttonRect, iconStyle, customAssetToAdd, AssetDatabase.GetAssetPath(assetInfo.mainAsset), false, null);
                    }
                }
                if (isDisabled)
                {
                    if (customAssetToAdd == null)
                    {
                        EditorGUILayout.HelpBox("Select an asset that can be a sub-asset of the parent asset", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("The selected asset cannot be a sub-asset of the parent asset", MessageType.Error);
                    }
                }
            }
        }
    }
}