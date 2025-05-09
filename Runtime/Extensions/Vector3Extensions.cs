using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public enum VectorField { X = 0, Y = 1, Z = 2 }

    public static Vector3 NormalizedWithCutoff(this Vector3 vector, float minMagnitude)
    {
        if (vector.sqrMagnitude < minMagnitude * minMagnitude)
        {
            return Vector3.zero;
        }
        return vector.normalized;
    }

    public static Vector3 ProjectOnSimplePlane(this Vector3 vector, VectorField planeNormalDirection)
    {
        Vector3 normal = new Vector3() { [(int)planeNormalDirection] = 1 };
        return Vector3.Scale(vector, Vector3.one - normal);
    }
}
