using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
{
    public static T Instance 
    { 
        get
        {
            if (!isInstanced)
            {
                T[] assets = Resources.LoadAll<T>("");
                if (assets == null || assets.Length < 1)
                {
                    Debug.LogError($"Couldn't find singleton {typeof(T).Name} in Resources folder");
                    return null;
                }
                else if (assets.Length > 1)
                {
                    Debug.LogWarning($"Multiple singletons {typeof(T).Name} found in Resources folder");
                }
                instance = assets[0];
                isInstanced = true;
            }
            return instance;
        }
    }

    private static T instance = null;
    private static bool isInstanced = false;
}
