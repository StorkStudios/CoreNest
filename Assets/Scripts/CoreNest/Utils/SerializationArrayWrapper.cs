using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
