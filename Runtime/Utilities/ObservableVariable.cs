using System.Collections.Generic;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    [System.Serializable]
    public class ObservableVariable<T>
    {
        public delegate void ValueChangedDelegate(T oldValue, T newValue);

        public event ValueChangedDelegate ValueChanged;

        public T Value
        {
            get => current;
            set
            {
                if (current.Equals(value))
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