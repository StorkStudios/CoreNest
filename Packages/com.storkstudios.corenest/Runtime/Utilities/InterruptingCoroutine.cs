using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptingCoroutine
{
    private readonly MonoBehaviour context;
    private readonly System.Action functionToCall;

    private Coroutine currentCoroutine = null;

    public InterruptingCoroutine(System.Action functionToCall, MonoBehaviour callContext)
    {
        context = callContext;
        this.functionToCall = functionToCall;
    }

    public void Start(float delayTime)
    {
        if (currentCoroutine != null)
        {
            Stop();
        }

        currentCoroutine = context.StartCoroutine(CallCoroutine(delayTime, functionToCall));
    }

    public void Stop()
    {
        if (currentCoroutine != null)
        {
            context.StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    private IEnumerator CallCoroutine(float delayTime, System.Action delayedFunction)
    {
        yield return new WaitForSeconds(delayTime);

        delayedFunction?.Invoke();

        currentCoroutine = null;
    }
}

public class InterruptingCoroutine<T>
{
    private readonly MonoBehaviour context;
    private readonly System.Action<T> functionToCall;

    private Coroutine currentCoroutine = null;

    public InterruptingCoroutine(System.Action<T> functionToCall, MonoBehaviour callContext)
    {
        context = callContext;
        this.functionToCall = functionToCall;
    }

    public void Start(float delayTime, T parameter)
    {
        if (currentCoroutine != null)
        {
            Stop();
        }

        currentCoroutine = context.StartCoroutine(CallCoroutine(delayTime, functionToCall, parameter));
    }

    public void Stop()
    {
        if (currentCoroutine != null && context != null)
        {
            context.StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    private IEnumerator CallCoroutine(float delayTime, System.Action<T> delayedFunction, T parameter)
    {
        yield return new WaitForSeconds(delayTime);

        delayedFunction?.Invoke(parameter);

        currentCoroutine = null;
    }
}