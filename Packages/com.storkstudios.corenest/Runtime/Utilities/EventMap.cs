using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMap<T>
{
    private Dictionary<T, EventHandlerWrapper> eventDictionary = new Dictionary<T, EventHandlerWrapper>();

    public void AddListener(T key, System.Action action)
    {
        if (!eventDictionary.ContainsKey(key))
        {
            eventDictionary.Add(key, new EventHandlerWrapper());
        }
        //Remove potential duplicate listener
        eventDictionary[key] -= action;
        eventDictionary[key] += action;
    }

    public void RemoveListener(T key, System.Action action)
    {
        if (!eventDictionary.ContainsKey(key))
        {
            return;
        }

        eventDictionary[key] -= action;
        if (eventDictionary[key].IsEmpty())
        {
            eventDictionary.Remove(key);
        }
    }

    public void Invoke(T key)
    {
        if (!eventDictionary.ContainsKey(key))
        {
            return;
        }
        eventDictionary[key].Invoke();
    }
}


public class EventMap<T, U>
{
    private Dictionary<T, EventHandlerWrapper<U>> eventDictionary = new Dictionary<T, EventHandlerWrapper<U>>();

    public void AddListener(T key, System.Action<U> action)
    {
        if (!eventDictionary.ContainsKey(key))
        {
            eventDictionary.Add(key, new EventHandlerWrapper<U>());
        }
        //Remove potential duplicate listener
        eventDictionary[key] -= action;
        eventDictionary[key] += action;
    }

    public void RemoveListener(T key, System.Action<U> action)
    {
        if (!eventDictionary.ContainsKey(key))
        {
            return;
        }

        eventDictionary[key] -= action;
        if (eventDictionary[key].IsEmpty())
        {
            eventDictionary.Remove(key);
        }
    }

    public void Invoke(T key, U arg)
    {
        if (!eventDictionary.ContainsKey(key))
        {
            return;
        }
        eventDictionary[key].Invoke(arg);
    }
}
