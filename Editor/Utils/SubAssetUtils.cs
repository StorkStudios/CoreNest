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

        private static bool IsBuiltinAsset(Object asset)
        {
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out _) &&
                   Regex.IsMatch(guid, "0000000000000000[0-9a-f]000000000000000");
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
            AssetDatabase.RemoveObjectFromAsset(asset);
            AssetDatabase.AddObjectToAsset(asset, parentPath);
            if (AssetDatabase.LoadAllAssetsAtPath(path).Length == 0)
            {
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}