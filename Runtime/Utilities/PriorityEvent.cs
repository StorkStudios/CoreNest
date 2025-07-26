using System.Collections.Generic;

namespace StorkStudios.CoreNest
{
    public class PriorityEvent
    {
        private readonly SortedDictionary<int, System.Action> callbacks = new SortedDictionary<int, System.Action>();

        public void Subscribe(int priority, System.Action callback)
        {
            if (!callbacks.ContainsKey(priority))
            {
                callbacks.Add(priority, null);
            }
            callbacks[priority] += callback;
        }

        public void Unsubscribe(int priority, System.Action callback)
        {
            callbacks[priority] -= callback;
            if (callbacks[priority] == null)
            {
                callbacks.Remove(priority);
            }
        }

        public void UnsubscribeAll(System.Action callback)
        {
            foreach (int key in callbacks.Keys)
            {
                callbacks[key] -= callback;
                if (callbacks[key] == null)
                {
                    callbacks.Remove(key);
                }
            }
        }

        public void Invoke()
        {
            foreach (System.Action callback in callbacks.Values)
            {
                callback?.Invoke();
            }
        }
    }

    public class PriorityEvent<T>
    {
        private readonly SortedDictionary<int, System.Action<T>> callbacks = new SortedDictionary<int, System.Action<T>>();

        public void Subscribe(int priority, System.Action<T> callback)
        {
            if (!callbacks.ContainsKey(priority))
            {
                callbacks.Add(priority, null);
            }
            callbacks[priority] += callback;
        }

        public void Unsubscribe(int priority, System.Action<T> callback)
        {
            callbacks[priority] -= callback;
            if (callbacks[priority] == null)
            {
                callbacks.Remove(priority);
            }
        }

        public void UnsubscribeAll(System.Action<T> callback)
        {
            foreach (int key in callbacks.Keys)
            {
                callbacks[key] -= callback;
                if (callbacks[key] == null)
                {
                    callbacks.Remove(key);
                }
            }
        }

        public void Invoke(T arg)
        {
            foreach (System.Action<T> callback in callbacks.Values)
            {
                callback?.Invoke(arg);
            }
        }
    }
}