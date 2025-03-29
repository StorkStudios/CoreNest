using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;

// unity cant cope with class that inherits from hash set bc of reasons so this class just mimics its set property hence all the interfaces
[System.Serializable]
public class SerializedSet<T> : ICollection<NullableObject<T>>, IEnumerable<NullableObject<T>>, IEnumerable, IReadOnlyCollection<NullableObject<T>>, ISet<NullableObject<T>>, IDeserializationCallback, ISerializable, ISerializationCallbackReceiver
{
    private HashSet<NullableObject<T>> set;

    [SerializeField]
    private List<NullableObject<T>> list = new List<NullableObject<T>>();

    public void OnAfterDeserialize()
    {
        set.Clear();
        foreach (NullableObject<T> item in list)
        {
            if (set.Contains(item))
            {
                set.Add(default(T));
            }
            else
            {
                set.Add(item);
            }
        }
    }

    public void OnBeforeSerialize()
    {
        list.Clear();
        foreach (NullableObject<T> item in set)
        {
            list.Add(item);
        }
    }

    // just code that gets send to set bc unity doesnt work when inheriting from hashset
    public SerializedSet() { set = new HashSet<NullableObject<T>>(); }
    public SerializedSet(IEnumerable<T> collection) { set = new HashSet<NullableObject<T>>(collection.Cast<NullableObject<T>>()); }
    public SerializedSet(IEqualityComparer<NullableObject<T>> comparer) { set = new HashSet<NullableObject<T>>(comparer); }
    public SerializedSet(IEnumerable<T> collection, IEqualityComparer<NullableObject<T>> comparer) { set = new HashSet<NullableObject<T>>(collection.Cast<NullableObject<T>>(), comparer); }

    public int Count => set.Count;
    public bool IsReadOnly => ((ICollection<NullableObject<T>>)set).IsReadOnly;
    public void Add(NullableObject<T> item) { set.Add(item); }
    public void Clear() { set.Clear(); }
    public bool Contains(NullableObject<T> item) { return set.Contains(item); }
    public void CopyTo(NullableObject<T>[] array, int arrayIndex) { set.CopyTo(array, arrayIndex); }
    public void ExceptWith(IEnumerable<NullableObject<T>> other) { set.ExceptWith(other); }
    public IEnumerator<NullableObject<T>> GetEnumerator() { return set.GetEnumerator(); }
    public void GetObjectData(SerializationInfo info, StreamingContext context) { set.GetObjectData(info, context); }
    public void IntersectWith(IEnumerable<NullableObject<T>> other) { set.IntersectWith(other); }
    public bool IsProperSubsetOf(IEnumerable<NullableObject<T>> other) { return set.IsProperSubsetOf(other); }
    public bool IsProperSupersetOf(IEnumerable<NullableObject<T>> other) { return set.IsProperSupersetOf(other); }
    public bool IsSubsetOf(IEnumerable<NullableObject<T>> other) { return set.IsSubsetOf(other); }
    public bool IsSupersetOf(IEnumerable<NullableObject<T>> other) { return set.IsSupersetOf(other); }
    public void OnDeserialization(object sender) { set.OnDeserialization(sender); }
    public bool Overlaps(IEnumerable<NullableObject<T>> other) { return set.Overlaps(other); }
    public bool Remove(NullableObject<T> item) { return set.Remove(item); }
    public bool SetEquals(IEnumerable<NullableObject<T>> other) { return set.SetEquals(other); }
    public void SymmetricExceptWith(IEnumerable<NullableObject<T>> other) { set.SymmetricExceptWith(other); }
    public void UnionWith(IEnumerable<NullableObject<T>> other) { set.UnionWith(other); }
    bool ISet<NullableObject<T>>.Add(NullableObject<T> item) { return set.Add(item); }
    IEnumerator IEnumerable.GetEnumerator() { return set.GetEnumerator(); }
}
