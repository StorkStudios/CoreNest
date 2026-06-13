using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Class for storing value and emitting events when value is changed.
    /// Change is detected using <see cref="object.Equals(object, object)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the stored value</typeparam>
    [System.Serializable]
    public class ObservableVariable<T> : IReadOnlyObservableVariable<T>
    {
        public delegate void ValueChangedDelegate(T oldValue, T newValue);

        /// <summary>
        /// This event is emitted after setting the stored value to a different new value.
        /// </summary>
        public event ValueChangedDelegate ValueChanged;

        /// <summary>
        /// Access to the stored value.
        /// When the value is set and a change is detected using <see cref="object.Equals(object, object)"/>
        /// the <see cref="ValueChanged"/> event is emitted.
        /// </summary>
        public T Value
        {
            get => current;
            set
            {
                if (Equals(current, value))
                {
                    return;
                }

                T oldValue = current;
                current = value;
                InvokeValueChanged(oldValue, current);
            }
        }

        private void InvokeValueChanged(T oldValue, T current)
        {
            ValueChanged?.Invoke(oldValue, current);
        }

        [SerializeField]
        private T current;

        public ObservableVariable()
        {
            current = default;
        }

        public ObservableVariable(T initialValue)
        {
            current = initialValue;
        }
    }
}