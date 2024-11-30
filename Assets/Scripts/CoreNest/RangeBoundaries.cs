using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum BoundariesType
{
    IncludeNone = 0,
    IncludeLeft = 1,
    IncludeRight = 2,
    IncludeBoth = IncludeLeft | IncludeRight
}

[System.Serializable]
public class RangeBoundaries<T>
{
    public T Min;
    public T Max;
}

[System.Serializable]
public class RangeBoundariesFloat : RangeBoundaries<float>
{
    public float GetRandomBetween()
    {
        return Random.Range(Min, Max);
    }

    public bool Contains(float value, BoundariesType boundariesType = BoundariesType.IncludeNone)
    {
        return (boundariesType.HasFlag(BoundariesType.IncludeLeft) ? value >= Min : value > Min) &&
            (boundariesType.HasFlag(BoundariesType.IncludeRight) ? value <= Max : value < Max);
    }

    public float GetAverage()
    {
        return (Min + Max) / 2;
    }
}
