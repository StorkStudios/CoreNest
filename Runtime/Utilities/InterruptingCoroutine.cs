using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper for a coroutine that executes a function after the specified time.
/// </summary>
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

    /// <summary>
    /// Stops the currently running coroutine (if there is one) and starts a new one with specified <paramref name="delayTime"/>.
    /// </summary>
    public void Start(float delayTime)
    {
        if (currentCoroutine != null)
        {
            Stop();
        }

        if (delayTime > 0)
        {
            currentCoroutine = context.StartCoroutine(CallCoroutine(delayTime, functionToCall));
        }
        else
        {
            functionToCall?.Invoke();
        }
    }

    /// <summary>
    /// Stops the currently running coroutine.
    /// </summary>
    /// <returns>Was the coroutine running.</returns>
    public bool Stop()
    {
        if (currentCoroutine != null)
        {
            context.StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            return true;
        }
        return false;
    }

    private IEnumerator CallCoroutine(float delayTime, System.Action delayedFunction)
    {
        yield return new WaitForSeconds(delayTime);

        delayedFunction?.Invoke();

        currentCoroutine = null;
    }
}

/// <summary>
/// Wrapper for a coroutine that executes a function after the specified time.
/// </summary>
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

    /// <summary>
    /// Stops the currently running coroutine (if there is one) and starts a new one with specified <paramref name="delayTime"/>.
    /// </summary>
    public void Start(float delayTime, T parameter)
    {
        if (currentCoroutine != null)
        {
            Stop();
        }

        if (delayTime > 0)
        {
            currentCoroutine = context.StartCoroutine(CallCoroutine(delayTime, functionToCall, parameter));
        }
        else
        {
            functionToCall?.Invoke(parameter);
        }
    }

    /// <summary>
    /// Stops the currently running coroutine.
    /// </summary>
    /// <returns>Was the coroutine running.</returns>
    public bool Stop()
    {
        if (currentCoroutine != null && context != null)
        {
            context.StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            return true;
        }
        return false;
    }

    private IEnumerator CallCoroutine(float delayTime, System.Action<T> delayedFunction, T parameter)
    {
        yield return new WaitForSeconds(delayTime);

        delayedFunction?.Invoke(parameter);

        currentCoroutine = null;
    }
}