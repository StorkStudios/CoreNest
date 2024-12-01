using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vector2XZ
{
    public float x;
    public float z;

    public readonly Vector3 AsVector3()
    {
        return new Vector3(x, 0 , z);
    }

    public Vector2XZ(Vector3 v3)
    {
        x = v3.x;
        z = v3.z;
    }

    public Vector2XZ(float x, float z)
    {
        this.x = x;
        this.z = z;
    }
}
