using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class SubAssetUtils
    {
        public static bool CanTypeBeSubAsset(System.Type type, bool checkEditorTypes)
        {
            if (!typeof(Object).IsAssignableFrom(type))
            {
                return false;
            }

            if (typeof(Component).IsAssignableFrom(type) ||
                typeof(GameObject).IsAssignableFrom(type) ||
                typeof(Shader).IsAssignableFrom(type))
            {
                return false;
            }

            if (!checkEditorTypes)
            {
                return true;
            }

            if (typeof(SceneAsset).IsAssignableFrom(type) ||
                typeof(DefaultAsset).IsAssignableFrom(type) ||
                typeof(MonoScript).IsAssignableFrom(type))
            {
                return false;
            }

            return true;
        }

        public static bool CanInstanceBeSubAsset(Object asset, string parentPath)
        {
            return asset != null &&
                   AssetDatabase.GetAssetPath(asset) != parentPath &&
                   !IsBuiltinAsset(asset);
        }

        public static bool IsBuiltinAsset(Object asset)
        {
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out _) &&
                   Regex.IsMatch(guid, "0000000000000000[0-9a-f]000000000000000");
        }

        public static bool IsSubAssetUsedInAsset(Object subAsset, Object asset)
        {
            SerializedProperty iterator = new SerializedObject(asset).GetIterator();

            while (iterator.Next(true))
            {
                if (iterator.propertyType != SerializedPropertyType.ObjectReference || iterator.objectReferenceValue == null)
                {
                    continue;
                }

                if (iterator.objectReferenceValue == subAsset)
                {
                    return true;
                }
            }

            return false;
        }

        public static Object MakeSubAssetCopy(Object asset, string parentPath)
        {
            Object copy = Object.Instantiate(asset);
            AssetDatabase.AddObjectToAsset(copy, parentPath);
            return copy;
        }

        public static void MakeIntoSubAsset(Object asset, string parentPath)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            if (AssetDatabase.IsMainAsset(asset))
            {
                Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                foreach (Object subAsset in assets)
                {
                    if (subAsset == asset)
                    {
                        Debug.Log("bulech");
                        continue;
                    }

                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    AssetDatabase.AddObjectToAsset(subAsset, parentPath);
                }
            }
            AssetDatabase.RemoveObjectFromAsset(asset);
            AssetDatabase.AddObjectToAsset(asset, parentPath);
            if (AssetDatabase.LoadAllAssetsAtPath(path).Length == 0)
            {
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}