using UnityEngine;

namespace StorkStudios.CoreNest
{
    public interface IMethodParameters
    {
        public abstract int Count { get; }
        public abstract object[] GetValues();
    }
}