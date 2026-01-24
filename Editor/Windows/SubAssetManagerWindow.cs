using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public class SubAssetManagerWindow : EditorWindow
    {
        private struct AssetInfo
        {
            public Object mainAsset;
            public Dictionary<Object, List<SerializedProperty>> subAssets;
            public Dictionary<Object, List<SerializedProperty>> dependencyAssets;
        }

        [SerializeField]
        private Object parentAsset;

        [SerializeField]
        private Object customAssetToAdd;

        [MenuItem("Tools/Core'Nest/Sub-asset manager")]
        private static void Open()
        {
            GetWindow(typeof(SubAssetManagerWindow));
        }

        private static AssetInfo GetAssetInfo(Object assetObject)
        {
            AssetInfo assetInfo;
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(assetObject));
            assetInfo.mainAsset = assets.First(e => AssetDatabase.IsMainAsset(e));
            assetInfo.subAssets = new Dictionary<Object, List<SerializedProperty>>();
            assetInfo.dependencyAssets = new Dictionary<Object, List<SerializedProperty>>();

            foreach (Object asset in assets)
            {
                if (!AssetDatabase.IsSubAsset(asset))
                {
                    continue;
                }

                assetInfo.subAssets.Add(asset, new List<SerializedProperty>());
            }

            foreach (Object asset in assets)
            {
                SerializedProperty iterator = new SerializedObject(asset).GetIterator();

                while (iterator.Next(true))
                {
                    if (iterator.propertyType != SerializedPropertyType.ObjectReference || iterator.objectReferenceValue == null)
                    {
                        continue;
                    }

                    Object objectValue = iterator.objectReferenceValue;
                    if (objectValue == assetInfo.mainAsset)
                    {
                        continue;
                    }

                    if (assetInfo.subAssets.ContainsKey(objectValue))
                    {
                        assetInfo.subAssets[objectValue].Add(iterator.Copy());
                    }

                    if (SubAssetUtils.CanTypeBeSubAsset(objectValue.GetType(), true))
                    {
                        if (!assetInfo.dependencyAssets.ContainsKey(objectValue))
                        {
                            assetInfo.dependencyAssets.Add(objectValue, new List<SerializedProperty>());
                        }
                        assetInfo.dependencyAssets[objectValue].Add(iterator.Copy());
                    }
                }
            }

            return assetInfo;
        }

        private static string GetAssetPropertyPaths(Object asset, List<SerializedProperty> properties)
        {
            if (properties.Count == 0)
            {
                return "";
            }

            bool first = true;
            StringBuilder sb = new StringBuilder();
            foreach (SerializedProperty property in properties)
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

            AssetInfo assetInfo = GetAssetInfo(parentAsset);
            parentAsset = assetInfo.mainAsset;

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Sub-assets", GUIStyles.Bold);
            using (new EditorGUI.IndentLevelScope(1))
            {
                IEnumerable<Object> subAssets = assetInfo.subAssets.Keys;
                if (subAssets.Count() <= 0)
                {
                    EditorGUILayout.HelpBox("No sub-assets in this asset", MessageType.Info);
                }
                foreach (Object subAsset in subAssets)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        float buttonWidth = EditorStyles.iconButton.fixedWidth;
                        Rect controlRect = EditorGUILayout.GetControlRect();
                        Rect rect = EditorGUI.IndentedRect(controlRect);

                        bool isSubAssetUsed = assetInfo.subAssets[subAsset].Count > 0;
                        string usedIcon = isSubAssetUsed ? "CacheServerConnected" : "CacheServerDisabled";
                        rect.xMax = rect.xMin + EditorStyles.iconButton.fixedWidth;
                        GUIContent usedContent = new GUIContent(EditorGUIUtility.IconContent(usedIcon));
                        usedContent.tooltip = isSubAssetUsed ? GetAssetPropertyPaths(subAsset, assetInfo.subAssets[subAsset]) : "This sub asset is not used in the parent asset";
                        GUI.Label(rect, usedContent, EditorStyles.iconButton);

                        rect.x += rect.width + EditorGUIUtility.standardVerticalSpacing;
                        GUIContent thumbnail = new GUIContent { image = AssetPreview.GetMiniThumbnail(subAsset) };
                        if (GUI.Button(rect, thumbnail, EditorStyles.iconButton))
                        {
                            EditorGUIUtility.PingObject(subAsset);
                        }

                        rect.xMin = controlRect.xMin + (buttonWidth + EditorGUIUtility.standardVerticalSpacing) * 2;
                        rect.xMax = controlRect.xMax - (buttonWidth + EditorGUIUtility.standardVerticalSpacing) * 1;
                        string newName = EditorGUI.DelayedTextField(rect, subAsset.name);
                        if (subAsset.name != newName)
                        {
                            subAsset.name = newName;
                            AssetDatabase.SaveAssets();
                        }

                        rect.xMin = rect.xMax + EditorGUIUtility.standardVerticalSpacing;
                        rect.xMax = rect.xMin + EditorStyles.iconButton.fixedWidth;
                        GUIContent deleteContent = new GUIContent(EditorGUIUtility.IconContent("P4_DeletedLocal@2x"));
                        deleteContent.tooltip = "Delete this sub-asset";
                        if (GUI.Button(rect, deleteContent, EditorStyles.iconButton) &&
                            EditorUtility.DisplayDialog("SubAsset: Destroy sub-asset", $"Are you sure you want to destroy sub-asset \"{subAsset.name}\"?. Consider making a copy.", "Destroy", "Cancel"))
                        {
                            DestroyImmediate(subAsset, true);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Assets used by the parent asset", GUIStyles.Bold);
            using (new EditorGUI.IndentLevelScope(1))
            {
                IEnumerable<Object> dependencyAssets = assetInfo.dependencyAssets.Keys.Where(e => !assetInfo.subAssets.ContainsKey(e));
                if (dependencyAssets.Count() <= 0)
                {
                    EditorGUILayout.HelpBox("No assets are used by this asset", MessageType.Info);
                }
                foreach (Object asset in dependencyAssets)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.ObjectField(asset, asset.GetType(), false);
                        }

                        GUIStyle iconStyle = new GUIStyle(EditorStyles.iconButton);
                        iconStyle.margin.top += 2;
                        Rect buttonRect = GUILayoutUtility.GetRect(EditorGUIUtility.IconContent("d__Menu@2x"), iconStyle);
                        if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d__Menu@2x"), iconStyle))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Make into sub-asset"), false, () =>
                            {
                                SubAssetUtils.MakeIntoSubAsset(asset, AssetDatabase.GetAssetPath(assetInfo.mainAsset));
                                AssetDatabase.SaveAssets();
                            });
                            menu.AddItem(new GUIContent("Create sub-asset copy"), false, () =>
                            {
                                Object copy = SubAssetUtils.MakeSubAssetCopy(asset, AssetDatabase.GetAssetPath(assetInfo.mainAsset));

                                foreach (SerializedProperty property in assetInfo.dependencyAssets[asset])
                                {
                                    property.objectReferenceValue = copy;
                                    property.serializedObject.ApplyModifiedProperties();
                                }

                                AssetDatabase.SaveAssets();
                            });
                           
                            menu.DropDown(buttonRect);
                        }
                    }
                }
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Add another asset", GUIStyles.Bold);
            using (new EditorGUI.IndentLevelScope(1))
            using (new EditorGUILayout.HorizontalScope())
            {
                customAssetToAdd = EditorGUILayout.ObjectField(customAssetToAdd, typeof(Object), false);

                GUIStyle iconStyle = new GUIStyle(EditorStyles.iconButton);
                iconStyle.margin.top += 2;
                Rect buttonRect = GUILayoutUtility.GetRect(EditorGUIUtility.IconContent("d__Menu@2x"), iconStyle);
                if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d__Menu@2x"), iconStyle) && customAssetToAdd != null)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Make into sub-asset"), false, () =>
                    {
                        SubAssetUtils.MakeIntoSubAsset(customAssetToAdd, AssetDatabase.GetAssetPath(assetInfo.mainAsset));
                        AssetDatabase.SaveAssets();
                    });
                    menu.AddItem(new GUIContent("Create sub-asset copy"), false, () =>
                    {
                        SubAssetUtils.MakeSubAssetCopy(customAssetToAdd, AssetDatabase.GetAssetPath(assetInfo.mainAsset));
                        AssetDatabase.SaveAssets();
                    });

                    menu.DropDown(buttonRect);
                }
            }
        }
    }
}