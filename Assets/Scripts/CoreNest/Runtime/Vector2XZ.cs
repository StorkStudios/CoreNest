using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vector2XZ
{
    public float x;
    public float z;

    public Vector3 AsVector3()
    {
        return new Vector3(x, 0 , z);
    }
}
