using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservableValue<T>
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

    public ObservableValue()
    {
        current = default;
    }

    public ObservableValue(T initialValue)
    {
        current = initialValue;
    }
}
