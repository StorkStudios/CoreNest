using System.Linq;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class AnimationCurveExtensions
    {
        [System.Flags]
        public enum NormalizationAxis
        {
            X = 1,
            Y = 2
        }

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

        public static AnimationCurve GetNormalizedAnimationCurve(this AnimationCurve curve, NormalizationAxis axes = NormalizationAxis.X | NormalizationAxis.Y)
        {
            AnimationCurve result = new();

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
                if (axes.HasFlag(NormalizationAxis.X))
                {
                    if (key.time < minKeyTime)
                    {
                        minKeyTime = key.time;
                    }
                    if (key.time > maxKeyTime)
                    {
                        maxKeyTime = key.time;
                    }
                }
                if (axes.HasFlag(NormalizationAxis.Y))
                {
                    if (key.value > maxKeyValue)
                    {
                        maxKeyValue = key.value;
                    }
                    if (key.value < minKeyValue)
                    {
                        minKeyValue = key.value;
                    }
                }
            }

            foreach (Keyframe key in curve.keys)
            {
                Keyframe normalizedKey = key;
                if (axes.HasFlag(NormalizationAxis.X))
                {
                    normalizedKey.time = Mathf.InverseLerp(minKeyTime, maxKeyTime, key.time);
                }
                if (axes.HasFlag(NormalizationAxis.Y))
                {
                    normalizedKey.value = Mathf.InverseLerp(minKeyValue, maxKeyValue, key.value);
                }
                result.AddKey(normalizedKey);
            }
            return result;
        }

        public static float GetDerivativeAt(this AnimationCurve curve, float time, float eps = 0.001f)
        {
            if (curve == null || curve.length < 1)
            {
                return 0f;
            }

            float start = curve.keys[0].time;
            float end = curve.keys[curve.length - 1].time;

            float x1 = Mathf.Max(start, time - eps);
            float x2 = Mathf.Min(end, time + eps);

            if (Mathf.Approximately(x1, x2))
            {
                return 0f;
            }

            return (curve.Evaluate(x2) - curve.Evaluate(x1)) / (x2 - x1);

        }
    }
}