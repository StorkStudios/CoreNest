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
    }
}