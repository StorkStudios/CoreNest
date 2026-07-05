using UnityEngine;
using UnityEngine.Events;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// A ScriptableObject that can be used to trigger a <see cref="Trigger"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "GlobalTrigger", menuName = "Core'Nest/GlobalTrigger")]
    public class GlobalTrigger : ScriptableObject
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
    /// A ScriptableObject that can be used to trigger a <see cref="Trigger{T}"/>.
    /// Due to inability to create assets of generic types, this class is abstract
    /// and must be inherited from to create a concrete type.
    /// </summary>
    public abstract class GlobalTrigger<T> : ScriptableObject
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