using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AnimationCurveExtensions
{
    public static float EvaluateUnclamped(this AnimationCurve curve, float time)
    {
        Keyframe first = curve.keys.First();
        Keyframe last = curve.keys.Last();
        if (first.time <= time && time <= last.time)
        {
            return curve.Evaluate(time);
        }
        return time < first.time ? first.value : last.value;
    }
}
