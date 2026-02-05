using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Utility functions for managing sub-assets inside the editor
    /// </summary>
    public static class SubAssetUtils
    {
        /// <summary>
        /// Determines whether the specified type can be used as a sub-asset in a Unity project.
        /// </summary>
        /// <remarks>
        /// Types derived from<br/>
        /// - runtime: Component, GameObject, Shader<br/>
        /// - editor: SceneAsset, DefaultAsset, MonoScript<br/>
        /// are considered invalid sub-asset types.
        /// </remarks>
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

        /// <summary>
        /// Determines whether the specified asset instance can be assigned as a sub-asset of the asset at the given parent path.
        /// </summary>
        public static bool CanInstanceBeSubAsset(Object asset, string parentPath)
        {
            return asset != null &&
                   AssetDatabase.GetAssetPath(asset) != parentPath &&
                   !IsBuiltinAsset(asset);
        }

        /// <summary>
        /// Determines whether the specified asset is a built-in Unity asset.
        /// </summary>
        /// <remarks>
        /// Built-in assets are internal Unity resources that are not user-created or stored as files in the project.
        /// This function indentifies them by checking for a spoofed asset GUID.
        /// </remarks>
        public static bool IsBuiltinAsset(Object asset)
        {
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out _) &&
                   Regex.IsMatch(guid, "0000000000000000[0-9a-f]000000000000000");
        }

        /// <summary>
        /// Determines whether the specified sub-asset is referenced by the given asset.
        /// </summary>
        /// <remarks>
        /// This method inspects all object reference properties of the asset to check for direct
        /// references to the specified sub-asset. Only direct object reference properties are considered; references
        /// within collections or nested objects may not be detected.
        /// </remarks>
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

        /// <summary>
        /// Creates a copy of the specified asset and adds it as a sub-asset to the asset at the given path.
        /// </summary>
        /// <remarks>
        /// This function doesn't call <see cref="AssetDatabase.SaveAssets"/>, so the changes won't be persisted until that function is called.
        /// </remarks>
        public static Object MakeSubAssetCopy(Object asset, string parentPath)
        {
            Object copy = Object.Instantiate(asset);
            AssetDatabase.AddObjectToAsset(copy, parentPath);
            return copy;
        }

        /// <summary>
        /// Moves the specified asset into a new parent asset at the given path.
        /// </summary>
        /// <remarks>
        /// If the asset is a main asset, all of its sub-assets are also moved to the new parent.<br/>
        /// This function doesn't call <see cref="AssetDatabase.SaveAssets"/>, so the changes won't be persisted until that function is called.
        /// </remarks>
        public static void MoveIntoSubAsset(Object asset, string parentPath)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            if (AssetDatabase.IsMainAsset(asset))
            {
                Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                foreach (Object subAsset in assets)
                {
                    if (subAsset == asset)
                    {
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

        /// <summary>
        /// Determines whether the specified asset is a sub-asset located at the given path.
        /// </summary>
        public static bool IsSubAssetOf(Object asset, string parentPath)
        {
            return AssetDatabase.IsSubAsset(asset) && AssetDatabase.GetAssetPath(asset) == parentPath;
        }
    }
}