using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StorkStudios.CoreNest
{
    public class FindScriptWindow : EditorWindow
    {
        [SerializeField]
        private List<string> assetPaths = new List<string>();
        [SerializeField]
        private List<GameObject> objectsInScene = new List<GameObject>();
        [SerializeField]
        private MonoScript script;

        private GUIStyle boxStyle = null;

        [MenuItem("Tools/Core'Nest/Find objects with script")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindScriptWindow));
        }

        private void OnGUI()
        {
            GUILayout.Label("Find objects with script", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("Script to find (leave empty for missing script search)");
            script = EditorGUILayout.ObjectField(script, typeof(MonoScript), false) as MonoScript;
            GUILayout.Space(10);

            boxStyle ??= MakeBoxStyle();

            using (new EditorGUILayout.VerticalScope(boxStyle))
            {
                if (GUILayout.Button("Find in current scene"))
                {
                    FindScriptsInCurrentScene();
                }
                GUILayout.Label("Results (Open Scenes):", EditorStyles.boldLabel);

                objectsInScene.RemoveAll(o => !o);

                foreach (GameObject gameObject in objectsInScene)
                {
                    if (GUILayout.Button(gameObject.name))
                    {
                        EditorGUIUtility.PingObject(gameObject);
                    }
                }
            }

            GUILayout.Space(10);

            using (new EditorGUILayout.VerticalScope(boxStyle))
            {
                if (GUILayout.Button("Find in assets"))
                {
                    FindScriptInAssets();
                }
                GUILayout.Label("Results (Assets):", EditorStyles.boldLabel);
                foreach (string path in assetPaths)
                {
                    if (GUILayout.Button(path))
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
                    }
                }
            }
        }

        private GUIStyle MakeBoxStyle()
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.18f, 0.18f, 0.18f));
            return boxStyle;
        }

        private static Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        private void FindScriptsInCurrentScene()
        {
            objectsInScene.Clear();
            IEnumerable<GameObject> rootObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(o => o.transform.parent == null);
            foreach (GameObject gameObject in rootObjects)
            {
                FindScriptsInGameObjectAndChildren(gameObject);
            }
        }

        private void FindScriptsInGameObjectAndChildren(GameObject gameObject)
        {
            bool hasScript = false;
            if (script == null)
            {
                Component[] components = gameObject.GetComponents<Component>();
                hasScript = components.Any(c => c == null);
            }
            else
            {
                hasScript = gameObject.GetComponents(script.GetClass()).Length > 0;
            }

            if (hasScript)
            {
                objectsInScene.Add(gameObject);
            }

            foreach (Transform child in gameObject.transform)
            {
                FindScriptsInGameObjectAndChildren(child.gameObject);
            }
        }

        private void FindScriptInAssets()
        {
            assetPaths.Clear();
            IEnumerable<string> pathsToSearch = AssetDatabase.GetAllAssetPaths().Where(path => !path.StartsWith("Packages/") && Path.GetExtension(path) == ".prefab");
            foreach (string assetPath in pathsToSearch)
            {
                GameObject assetRoot = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                bool hasScript = false;
                if (script == null)
                {
                    Component[] components = assetRoot.GetComponentsInChildren<Component>(true);
                    hasScript = components.Any(c => c == null);
                }
                else
                {
                    hasScript = assetRoot.GetComponentsInChildren(script.GetClass(), true).Length > 0;
                }

                if (hasScript)
                {
                    assetPaths.Add(assetPath);
                }
            }
        }
    }
}