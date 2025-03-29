using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 NormalizedWithCutoff(this Vector3 vector, float minMagnitude)
    {
        if (vector.sqrMagnitude < minMagnitude * minMagnitude)
        {
            return Vector3.zero;
        }
        return vector.normalized;
    }
}
