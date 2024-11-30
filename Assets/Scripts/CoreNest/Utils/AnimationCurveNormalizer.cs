using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveNormalizer
{
    public static AnimationCurve NormalizeAnimationCurve(AnimationCurve curve)
    {
        AnimationCurve result = new AnimationCurve();

        if (curve == null || curve.length < 1)
        {
            return result;
        }

        float minKeyTime;
        float maxKeyTime;
        minKeyTime = maxKeyTime = curve.keys[0].time;
        float minKeyValue;
        float maxKeyValue;
        minKeyValue = maxKeyValue = curve.keys[0].value;

        foreach (Keyframe key in curve.keys[1..])
        {
            if (key.time < minKeyTime)
            {
                minKeyTime = key.time;
            }
            if (key.time > maxKeyTime)
            {
                maxKeyTime = key.time;
            }
            if (key.value > maxKeyValue)
            {
                maxKeyValue = key.value;
            }
            if (key.value < minKeyValue)
            {
                minKeyValue = key.value;
            }
        }

        foreach (Keyframe key in curve.keys)
        {
            Keyframe normalizedKey = key;
            normalizedKey.time = Mathf.InverseLerp(minKeyTime, maxKeyTime, key.time);
            normalizedKey.value = Mathf.InverseLerp(minKeyValue, maxKeyValue, key.value);
            result.AddKey(normalizedKey);
        }
        return result;
    }
}
