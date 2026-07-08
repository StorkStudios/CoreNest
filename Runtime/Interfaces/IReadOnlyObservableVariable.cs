using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Interface for exposing <see cref="ObservableVariable{T}"/> without the value setter.
    /// </summary>
    public interface IReadOnlyObservableVariable<T>
    {
        public event ObservableVariable<T>.ValueChangedDelegate ValueChanged;
        public T Value { get; }
    }
}