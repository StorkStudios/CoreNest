using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedQueue<T> : Queue<T>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<T> values = new List<T>();

    public void OnAfterDeserialize()
    {
        Clear();

        foreach (T item in values)
        {
            Enqueue(item);
        }
    }

    public void OnBeforeSerialize()
    {
        values = new List<T>(ToArray());
    }

    public SerializedQueue() : base() { }
    public SerializedQueue(int capacity) : base(capacity) { }
    public SerializedQueue(IEnumerable<T> collection) : base(collection) { }
}
