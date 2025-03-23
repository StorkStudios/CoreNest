using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static event Action<T> Initialized;

    public static T Instance 
    {
        get => instance;
    }

    private static T instance;

    private static bool isInitialized = false;

    public static void CallOnInitialize(Action<T> action)
    {
        if (isInitialized)
        {
            action.Invoke(instance);
        }
        else
        {
            Initialized.SubscribeOneShot(action);
        }
    }
}
