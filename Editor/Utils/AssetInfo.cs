using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public partial class SubAssetManagerWindow
    {
        private class AssetInfo
        {
            public Object mainAsset;
            public Dictionary<Object, List<SerializedProperty>> subAssets;
            public Dictionary<Object, List<SerializedProperty>> dependencyAssets;
            public Dictionary<Object, List<Object>> assetUsages;

            public AssetInfo(Object assetObject)
            {
                string path = AssetDatabase.GetAssetPath(assetObject);
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                mainAsset = assets.First(e => AssetDatabase.IsMainAsset(e));
                subAssets = new Dictionary<Object, List<SerializedProperty>>();
                dependencyAssets = new Dictionary<Object, List<SerializedProperty>>();
                assetUsages = new Dictionary<Object, List<Object>>();

                foreach (Object asset in assets)
                {
                    if (!AssetDatabase.IsSubAsset(asset))
                    {
                        continue;
                    }

                    subAssets.Add(asset, new List<SerializedProperty>());
                    assetUsages.Add(asset, new List<Object>());
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
                        if (objectValue == mainAsset)
                        {
                            continue;
                        }

                        if (subAssets.ContainsKey(objectValue))
                        {
                            subAssets[objectValue].Add(iterator.Copy());
                            assetUsages[objectValue].Add(asset);
                        }

                        if (AssetDatabase.GetAssetPath(objectValue) != path && SubAssetUtils.CanTypeBeSubAsset(objectValue.GetType(), true))
                        {
                            if (!dependencyAssets.ContainsKey(objectValue))
                            {
                                dependencyAssets.Add(objectValue, new List<SerializedProperty>());
                            }
                            dependencyAssets[objectValue].Add(iterator.Copy());
                        }
                    }
                }
            }

            public bool IsAssetUsedByMainAsset(Object asset)
            {
                if (asset == mainAsset)
                {
                    return true;
                }
                return assetUsages.ContainsKey(asset) && assetUsages[asset].Where(e => e != asset).Any(IsAssetUsedByMainAsset);
            }
        }
    }
}