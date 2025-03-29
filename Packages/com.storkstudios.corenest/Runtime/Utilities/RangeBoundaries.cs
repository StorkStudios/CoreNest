using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum BoundariesType
{
    IncludeNone = 0,
    IncludeMin = 1,
    IncludeMax = 2,
    IncludeBoth = IncludeMin | IncludeMax
}

[System.Serializable]
public class RangeBoundaries<T> where T : IComparable<T>
{
    public T Min;
    public T Max;

    public bool Contains(T value, BoundariesType boundariesType = BoundariesType.IncludeNone)
    {
        int minComp = value.CompareTo(Min);
        int maxComp = Max.CompareTo(value);

        return (boundariesType.HasFlag(BoundariesType.IncludeMin) ? minComp >= 0 : minComp > 0) &&
            (boundariesType.HasFlag(BoundariesType.IncludeMax) ? maxComp >= 0 : maxComp > 0);
    }
}

[System.Serializable]
public class RangeBoundariesFloat : RangeBoundaries<float>
{
    public float GetRandomBetween()
    {
        return UnityEngine.Random.Range(Min, Max);
    }

    public float GetAverage()
    {
        return (Min + Max) / 2;
    }

    public float NormalizeValue(float value)
    {
        return (value - Min) / (Max - Min);
    }
}
