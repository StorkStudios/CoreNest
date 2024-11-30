using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyComponentReference<T> where T : Component
{
    private T component;
    private bool hasValue = false;
    private MonoBehaviour context;

    public LazyComponentReference(MonoBehaviour context)
    {
        this.context = context;
    }

    public T GetValue()
    {
        if (!hasValue)
        {
            component = context.GetComponent<T>();
            hasValue = true;
        }
        return component;
    }
}
