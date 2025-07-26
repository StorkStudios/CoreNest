using System.Collections.Generic;

namespace StorkStudios.CoreNest
{
    public class ObservableVariable<T>
    {
        public delegate void ValueChangedDelegate(T oldValue, T newValue);

        public event ValueChangedDelegate ValueChanged;

        public T Value
        {
            get => current;
            set
            {
                if (EqualityComparer<T>.Default.Equals(current, value))
                {
                    return;
                }

                T oldValue = current;
                current = value;
                ValueChanged?.Invoke(oldValue, current);
            }
        }

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