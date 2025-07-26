using System.Collections.Generic;

namespace StorkStudios.CoreNest
{
    public class EventMap<TKey>
    {
        private Dictionary<TKey, EventHandlerWrapper> eventDictionary = new Dictionary<TKey, EventHandlerWrapper>();

        public void AddListener(TKey key, System.Action action)
        {
            if (!eventDictionary.ContainsKey(key))
            {
                eventDictionary.Add(key, new EventHandlerWrapper());
            }
            //Remove potential duplicate listener
            eventDictionary[key] -= action;
            eventDictionary[key] += action;
        }

        public void RemoveListener(TKey key, System.Action action)
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

        public void Invoke(TKey key)
        {
            if (!eventDictionary.ContainsKey(key))
            {
                return;
            }
            eventDictionary[key].Invoke();
        }
    }


    public class EventMap<TKey, TArg>
    {
        private Dictionary<TKey, EventHandlerWrapper<TArg>> eventDictionary = new Dictionary<TKey, EventHandlerWrapper<TArg>>();

        public void AddListener(TKey key, System.Action<TArg> action)
        {
            if (!eventDictionary.ContainsKey(key))
            {
                eventDictionary.Add(key, new EventHandlerWrapper<TArg>());
            }
            //Remove potential duplicate listener
            eventDictionary[key] -= action;
            eventDictionary[key] += action;
        }

        public void RemoveListener(TKey key, System.Action<TArg> action)
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

        public void Invoke(TKey key, TArg arg)
        {
            if (!eventDictionary.ContainsKey(key))
            {
                return;
            }
            eventDictionary[key].Invoke(arg);
        }
    }
}