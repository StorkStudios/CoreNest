using System.Collections.Generic;

namespace StorkStudios.CoreNest
{
    [System.Serializable]
    public class SerializedPair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public SerializedPair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public static implicit operator KeyValuePair<TKey, TValue>(SerializedPair<TKey, TValue> serializedPair)
        {
            return new KeyValuePair<TKey, TValue>(serializedPair.Key, serializedPair.Value);
        }

        public static implicit operator SerializedPair<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
        {
            return new SerializedPair<TKey, TValue>(pair.Key, pair.Value);
        }

        public void Deconstruct(out TKey key, out TValue value)
        {
            key = Key;
            value = Value;
        }
    }
}