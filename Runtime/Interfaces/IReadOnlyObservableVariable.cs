using UnityEngine;

namespace StorkStudios.CoreNest
{
    public interface IReadOnlyObservableVariable<T>
    {
        public event ObservableVariable<T>.ValueChangedDelegate ValueChanged;
        public T Value { get; }
    }
}