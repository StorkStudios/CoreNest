using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionHelper
{
    //This method works like regular Quaternion.LookRotation,
    //but prioritises upwards vector instead of forward
    public static Quaternion LookRotationUpwardPriority(Vector3 forward, Vector3 up)
    {
        Quaternion result = Quaternion.LookRotation(up, forward);
        return result * Quaternion.Euler(-90, 0, 180);
    }
}
