using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    public static T Instance
    {
        get
        {
            if (!IsInstanced)
            {
                T inst = FindAnyObjectByType<T>();
                SetOrCreateInstance(inst);
            }

            return instance;
        }
    }

    private static T instance;

    public static bool IsInstanced { get; private set; } = false;

    private static void SetOrCreateInstance(T inst)
    {
        if (IsInstanced && instance != inst && inst != null)
        {
            Debug.LogError($"More than one instance of singleton {typeof(T).Name} registered. First: {instance.gameObject.name}, second: {inst.gameObject.name}");
            return;
        }

        if (inst == null)
        {
            // awake happens during during gameobject construction
            inst = new GameObject($"{typeof(T).Name}").AddComponent<T>();
        }
        else if (inst.GetComponents<Component>().Where(e => !(e is T || e is Transform)).Count() > 0)
        {
            Debug.LogWarning($"Game object '{inst.name}' with '{typeof(T).Name}' persistent singleton contains additional components. " +
                "This object is moved to DontDestroyOnLoad scene so other componets could stop working.");
        }

        DontDestroyOnLoad(inst);
        instance = inst;
        IsInstanced = true;
    }

    protected virtual void Awake()
    {
        SetOrCreateInstance(this as T);
    }

    protected virtual void OnDestroy()
    {
        IsInstanced = false;
        instance = null;
    }
}
