using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static event Action<T> OnInitialize;

    public static T Instance 
    {
        get
        {
            if (!IsInstanced)
            {
                T inst = FindAnyObjectByType<T>();
                if (inst != null)
                {
                    RegisterInstance(inst);
                }
                else
                {
                    Debug.LogWarning($"Couldn't find {typeof(T).Name} singleton");
                }
            }

            return instance;
        }
    }

    private static T instance;

    public static bool IsInitialized { get; private set; } = false;
    public static bool IsInstanced { get; private set; } = false;

    public static void CallWhenInitialized(Action<T> action)
    {
        if (IsInitialized)
        {
            action.Invoke(instance);
        }
        else
        {
            void OneShot(T arg)
            {
                action?.Invoke(arg);
                OnInitialize -= OneShot;
            }

            OnInitialize += OneShot;
        }
    }

    private static void RegisterInstance(T inst)
    {
        if (IsInstanced && instance != inst)
        {
            Debug.LogError($"More than one instance of singleton {typeof(T).Name} registered. First: {instance.gameObject.name}, second: {inst.gameObject.name}");
        }
        else
        {
            instance = inst;
            IsInstanced = true;
        }
    }

    protected virtual void Awake()
    {
        RegisterInstance(this as T);
        IsInitialized = true;
        OnInitialize?.Invoke(instance);
    }

    protected virtual void OnDestroy()
    {
        IsInitialized = false;
        IsInstanced = false;
        instance = null;
    }
}
