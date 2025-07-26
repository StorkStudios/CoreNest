using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Helper component that converts animation event to unity event. Has a dictionary of events that are called by animation event ExternalEvent with string argument being the key.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationEventsConverter : MonoBehaviour
    {
        [Header("Event function name: ExternalEvent")]
        [SerializeField]
        private SerializedDictionary<string, UnityEvent> events;

        /// <summary>
        /// Function called by the animator
        /// </summary>
        /// <param name="key"></param>
        private void ExternalEvent(string key)
        {
            if (events.ContainsKey(key))
            {
                events[key].Invoke();
            }
            else
            {
                Debug.LogWarning($"No unity event with key {key} in {gameObject.name}:{nameof(AnimationEventsConverter)}");
            }
        }

        /// <summary>
        /// Adds the <paramref name="action"/> to the event of <paramref name="key"/>. If the <paramref name="key"/> doesn't exist in the dictionary it is added and new event is created.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void AddListener(string key, UnityAction action)
        {
            if (!events.ContainsKey(key))
            {
                events.Add(key, new UnityEvent());
            }
            events[key].AddListener(action);
        }

        /// <summary>
        /// Removes the <paramref name="action"/> from the event of <paramref name="key"/>. This doesn't clean up empty events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void RemoveListener(string key, UnityAction action)
        {
            if (events.ContainsKey(key))
            {
                events[key].RemoveListener(action);
            }
        }
    }
}