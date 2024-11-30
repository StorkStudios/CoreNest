using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeedDistanceTimeConfig
{
    private enum Mode
    {
        [InspectorName("Distance and time")]
        Speed,
        [InspectorName("Speed and time")]
        Distance,
        [InspectorName("Speed and distance")]
        Time
    }

    [SerializeField]
    private Mode mode;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float time;

    public float Speed => speed;
    public float Distance => distance;
    public float Time => time;
}
