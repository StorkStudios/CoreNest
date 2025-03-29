using System;
using UnityEngine;

public static class ActionExtensions
{
    public static void SubscribeOneShot(this Action self, Action action)
    {
        void InvokeOnce()
        {
            action?.Invoke();
            self -= action;
        }

        self += InvokeOnce;
    }

    public static void SubscribeOneShot<T>(this Action<T> self, Action<T> action)
    {
        void InvokeOnce(T arg)
        {
            action?.Invoke(arg);
            self -= action;
        }

        self += InvokeOnce;
    }
}
