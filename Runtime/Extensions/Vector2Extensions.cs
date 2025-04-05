using UnityEngine;

public static class Vector2Extensions
{
    public static Vector3 ToVector3(this Vector2 vector, float z = 0)
    {
        return new Vector3(vector.x, vector.y, z);
    }
}
