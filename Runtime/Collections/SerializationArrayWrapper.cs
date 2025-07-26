using UnityEngine;

namespace StorkStudios.CoreNest
{
    [System.Serializable]
    public class SerializationArrayWrapper<T>
    {
        [SerializeField]
        public T[] Array;

        public T this[int i]
        {
            get { return Array[i]; }
            set { Array[i] = value; }
        }
        public int Length { get { return Array.Length; } }
    }
}