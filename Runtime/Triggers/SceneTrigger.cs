using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// A component that can be used to trigger a <see cref="Trigger"/> on Scene objects.
    /// </summary>
    public class SceneTrigger : MonoBehaviour
    {
        [SerializeField]
        private Trigger trigger = new();

        public event UnityAction OnTrigger
        {
            add => trigger.AddListener(value);
            remove => trigger.RemoveListener(value);
        }

        public void Set()
        {
            trigger.Set(name);
        }
    }

    /// <summary>
    /// A component that can be used to trigger a <see cref="Trigger{T}"/> on Scene objects.
    /// Due to inability to create components of generic types, this class is abstract
    /// and must be inherited from to create a concrete type.
    /// </summary>
    public abstract class SceneTrigger<T> : MonoBehaviour
    {
        [SerializeField]
        private Trigger<T> trigger = new();

        public event UnityAction<T> OnTrigger
        {
            add => trigger.AddListener(value);
            remove => trigger.RemoveListener(value);
        }

        public void Set(T arg)
        {
            trigger.Set(name, arg);
        }
    }
}