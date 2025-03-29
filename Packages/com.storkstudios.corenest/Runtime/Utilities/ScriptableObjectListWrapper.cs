using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Objects of this class should not be created directly.
//You need to create a wrapper class and derive from this one.
public class ScriptableObjectListWrapper<T> : ScriptableObject
{
    [SerializeField]
    private List<T> list;

    public int Length => list.Count;

    public T this[int i]
    {
        get => list[i];
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public bool Contains(T value)
    {
        return list.Contains(value);
    }

    public HashSet<T> ToHashSet()
    {
        return list.ToHashSet();
    }
}
