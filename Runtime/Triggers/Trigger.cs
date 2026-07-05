using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// An <see cref="UnityEvent"/> that is used with trigger components.
    /// </summary>
    [System.Serializable]
    public class Trigger : UnityEvent
    {
        [SerializeField]
        private float delay;
        [SerializeField]
        private bool printDebugMessages;

        /// <summary>
        /// Sets the trigger.
        /// </summary>
        /// <param name="name">The name of the triggering object.</param>
        public void Set(string name)
        {
            if (delay > 0)
            {
                if (printDebugMessages)
                {
                    Debug.Log($"Trigger {name} Start Delay");
                }
                ActiveCoroutineContext.Instance.CallDelayed(delay, () =>
                {
                    if (printDebugMessages)
                    {
                        Debug.Log($"Trigger {name} Invoke");
                    }
                    Invoke();
                });
            }
            else
            {
                if (printDebugMessages)
                {
                    Debug.Log($"Trigger {name} Invoke");
                }
                Invoke();
            }
        }
    }


    /// <summary>
    /// An <see cref="UnityEvent{T0}"/> that is used with trigger components.
    /// </summary>
    [System.Serializable]
    public class Trigger<T> : UnityEvent<T>
    {
        [SerializeField]
        private float delay;
        [SerializeField]
        private bool printDebugMessages;

        /// <summary>
        /// Sets the trigger.
        /// </summary>
        /// <param name="name">The name of the triggering object.</param>
        public void Set(string name, T arg)
        {
            if (delay > 0)
            {
                if (printDebugMessages)
                {
                    Debug.Log($"Trigger {name} Start Delay");
                }
                ActiveCoroutineContext.Instance.CallDelayed(delay, () =>
                {
                    if (printDebugMessages)
                    {
                        Debug.Log($"Trigger {name} Invoke");
                    }
                    Invoke(arg);
                });
            }
            else
            {
                if (printDebugMessages)
                {
                    Debug.Log($"Trigger {name} Invoke");
                }
                Invoke(arg);
            }
        }
    }
}