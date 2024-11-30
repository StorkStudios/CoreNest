using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static Vector3 Average(this IEnumerable<Vector3> enumerable)
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 item in enumerable)
        {
            sum += item;
        }
        return sum / enumerable.Count();
    }

    public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
    {
        int i = Random.Range(0, enumerable.Count());
        return enumerable.ElementAt(i);
    }
}
