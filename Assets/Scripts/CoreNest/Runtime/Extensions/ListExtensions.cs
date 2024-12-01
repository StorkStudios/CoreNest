using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void ShuffleSelf<T>(this List<T> list)
    {
        int count = list.Count;
        for (int i = count - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
