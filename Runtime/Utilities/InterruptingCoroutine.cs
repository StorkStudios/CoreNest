using System;
using System.Collections;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Wrapper for a coroutine that contains utility functionality for its management.
    /// </summary>
    public class InterruptingCoroutine
    {
        private readonly MonoBehaviour context;
        private readonly Action functionToCall;

        private Coroutine currentCoroutine = null;

        public InterruptingCoroutine(Action functionToCall, MonoBehaviour callContext)
        {
            context = callContext;
            this.functionToCall = functionToCall;
        }

        /// <summary>
        /// Stops the currently running coroutine (if there is one) and calls the function immediately.
        /// </summary>
        public void Start()
        {
            Start(0);
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
                Start(new WaitForSeconds(delayTime));
            }
            else
            {
                functionToCall?.Invoke();
            }
        }

        /// <summary>
        /// Stops the currently running coroutine (if there is one) and starts a new one with specified <paramref name="delayInstruction"/>.
        /// </summary>
        public void Start(YieldInstruction delayInstruction)
        {
            IEnumerator DelayFunction() {
                yield return delayInstruction;
            }

            Start(DelayFunction);
        }

        /// <summary>
        /// Stops the currently running coroutine (if there is one) and starts a new one with specified <paramref name="delayFunction"/>.
        /// </summary>
        public void Start(Func<IEnumerator> delayFunction)
        {
            if (currentCoroutine != null)
            {
                Stop();
            }

            currentCoroutine = context.StartCoroutine(CallCoroutine(delayFunction, functionToCall));
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

        private IEnumerator CallCoroutine(Func<IEnumerator> delayFunction, Action delayedFunction)
        {
            yield return delayFunction?.Invoke();

            delayedFunction?.Invoke();

            currentCoroutine = null;
        }
    }

    /// <summary>
    /// Wrapper for a coroutine that contains utility functionality for its management.
    /// </summary>
    public class InterruptingCoroutine<T>
    {
        private readonly MonoBehaviour context;
        private readonly Action<T> functionToCall;

        private Coroutine currentCoroutine = null;

        public InterruptingCoroutine(Action<T> functionToCall, MonoBehaviour callContext)
        {
            context = callContext;
            this.functionToCall = functionToCall;
        }

        /// <summary>
        /// Stops the currently running coroutine (if there is one) and calls the function immediately.
        /// </summary>
        public void Start(T parameter)
        {
            Start(0, parameter);
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
                Start(new WaitForSeconds(delayTime), parameter);
            }
            else
            {
                functionToCall?.Invoke(parameter);
            }
        }

        /// <summary>
        /// Stops the currently running coroutine (if there is one) and starts a new one with specified <paramref name="delayInstruction"/>.
        /// </summary>
        public void Start(YieldInstruction delayInstruction, T parameter)
        {
            IEnumerator DelayFunction()
            {
                yield return delayInstruction;
            }

            Start(DelayFunction, parameter);
        }

        /// <summary>
        /// Stops the currently running coroutine (if there is one) and starts a new one with specified <paramref name="delayFunction"/>.
        /// </summary>
        public void Start(Func<IEnumerator> delayFunction, T parameter)
        {
            if (currentCoroutine != null)
            {
                Stop();
            }

            currentCoroutine = context.StartCoroutine(CallCoroutine(delayFunction, functionToCall, parameter));
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

        private IEnumerator CallCoroutine(Func<IEnumerator> delayFunction, Action<T> delayedFunction, T parameter)
        {
            yield return delayFunction?.Invoke();

            delayedFunction?.Invoke(parameter);

            currentCoroutine = null;
        }
    }
}