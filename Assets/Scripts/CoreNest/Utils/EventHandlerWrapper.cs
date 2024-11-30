using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandlerWrapper
{
    private event System.Action handler;

    public void Invoke()
    {
        handler?.Invoke();
    }

    public static EventHandlerWrapper operator +(EventHandlerWrapper handler, System.Action action)
    {
        handler.handler += action;
        return handler;
    }

    public static EventHandlerWrapper operator -(EventHandlerWrapper handler, System.Action action)
    {
        handler.handler -= action;
        return handler;
    }

    public bool IsEmpty()
    {
        return handler == null;
    }
}

public class EventHandlerWrapper<T>
{
    private event System.Action<T> handler;

    public void Invoke(T arg)
    {
        handler?.Invoke(arg);
    }

    public static EventHandlerWrapper<T> operator +(EventHandlerWrapper<T> handler, System.Action<T> action)
    {
        handler.handler += action;
        return handler;
    }

    public static EventHandlerWrapper<T> operator -(EventHandlerWrapper<T> handler, System.Action<T> action)
    {
        handler.handler -= action;
        return handler;
    }

    public bool IsEmpty()
    {
        return handler == null;
    }
}