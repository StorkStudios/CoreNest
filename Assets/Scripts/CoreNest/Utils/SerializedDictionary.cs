using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedDictionary<TKey, TValue> : Dictionary<NullableObject<TKey>, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SerializedPair<NullableObject<TKey>, TValue>> pairs = new List<SerializedPair<NullableObject<TKey>, TValue>>();

    public void OnAfterDeserialize()
    {
        Clear();
        for (int i = 0; i < pairs.Count; i++)
        {
            if (!ContainsKey(pairs[i].Key))
            {
                this[pairs[i].Key] = pairs[i].Value;
            }
            else
            {
                this[default(TKey)] = pairs[i].Value;
            }
        }
    }

    public void OnBeforeSerialize()
    {
        pairs.Clear();

        foreach (var item in this)
        {
            pairs.Add(item);
        }
    }
}