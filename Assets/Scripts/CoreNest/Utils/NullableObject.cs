using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public struct NullableObject<T>
{
    [SerializeField]
    private T item;

    public T Item => item;

    private NullableObject(T item) : this()
    {
        this.item = item;
    }

    public static NullableObject<T> Null()
    {
        return new NullableObject<T>();
    }

    public bool IsNull()
    {
        return item == null;
    }

    public static implicit operator T(NullableObject<T> nullObject)
    {
        return nullObject.item;
    }

    public static implicit operator NullableObject<T>(T item)
    {
        return new NullableObject<T>(item);
    }

    public override string ToString()
    {
        return (item != null) ? Item.ToString() : "NULL";
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return IsNull();
        }

        if (!(obj is NullableObject<T>))
        {
            return false;
        }

        var no = (NullableObject<T>)obj;

        if (IsNull())
        {
            return no.IsNull();
        }

        if (no.IsNull())
        {
            return false;
        }

        return Item.Equals(no.Item);
    }

    public override int GetHashCode()
    {
        if (IsNull())
        {
            return 0;
        }

        var result = Item.GetHashCode();

        if (result >= 0)
        {
            result++;
        }

        return result;
    }
}