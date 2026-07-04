using StorkStudios.CoreNest;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Trigger : UnityEvent
{
    [SerializeField]
    private float delay;
    [SerializeField]
    private bool printDebugMessages;

    public void Set(string name)
    {
        if (delay > 0)
        {
            if (printDebugMessages)
            {
                Debug.Log($"Trigger {name} Start Delay");
            }
            ActiveCoroutineContext.Instance.CallDelayed(delay, () => {
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

[System.Serializable]
public class Trigger<T> : UnityEvent<T>
{
    [SerializeField]
    private float delay;
    [SerializeField]
    private bool printDebugMessages;

    public void Set(string name, T arg)
    {
        if (delay > 0)
        {
            if (printDebugMessages)
            {
                Debug.Log($"Trigger {name} Start Delay");
            }
            ActiveCoroutineContext.Instance.CallDelayed(delay, () => {
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