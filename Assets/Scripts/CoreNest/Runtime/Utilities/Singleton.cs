using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static event Action OnInstantiate;
    public static bool IsInstanced => instance != null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    if (!isDestroyed)
                    {
                        Debug.LogWarning($"No existing singleton {typeof(T).Name}");
                    }
                }
                else
                {
                    InvokeOnInstantiate();
                }
            }
            return instance;
        }

        private set => instance = value;
    }

    private static T instance;

    private static bool isDestroyed = false;

    private void RegisterInstance(T inst)
    {
        if (instance != null && instance != inst)
        {
            Debug.LogError($"More than one instance of singleton {typeof(T).Name} registered. First: {instance.gameObject.name}, second: {inst.gameObject.name}");
        }
        else
        {
            instance = inst;
            InvokeOnInstantiate();
        }
    }

    private static void InvokeOnInstantiate()
    {
        isDestroyed = false;
        OnInstantiate?.Invoke();
    }

    protected virtual void Awake()
    {
        RegisterInstance(this as T);
    }

    protected virtual void OnDestroy()
    {
        isDestroyed = true;
        instance = null;
    }
}
