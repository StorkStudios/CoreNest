using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedLinkedList<T> : LinkedList<T>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<T> values = new List<T>();

    public void OnAfterDeserialize()
    {
        Clear();

        foreach (T item in values)
        {
            AddLast(item);
        }
    }

    public void OnBeforeSerialize()
    {
        values.Clear();
        Enumerator enumerator = GetEnumerator();
        while(enumerator.MoveNext())
        {
            values.Add(enumerator.Current);
        }
    }
}
