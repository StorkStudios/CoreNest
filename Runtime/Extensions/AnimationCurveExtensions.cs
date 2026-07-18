using System.Linq;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Evaluates the AnimationCurve at the specified time, returning the value of the curve at that time. If the time is outside the range of the curve's keyframes, it returns the value of the nearest keyframe (first or last) instead of extrapolating.
        /// </summary>
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

        /// <summary>
        /// Normalizes the AnimationCurve based on the specified axes (X and/or Y). The normalized curve will have its keyframe times and/or values scaled to fit within the range [0, 1].
        /// </summary>
        /// <returns>A new AnimationCurve instance with normalized keyframe times and/or values.</returns>
        public static AnimationCurve GetNormalizedAnimationCurve(this AnimationCurve curve, Axis2D axes = Axis2D.X | Axis2D.Y)
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
                if (axes.HasFlag(Axis2D.X))
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
                if (axes.HasFlag(Axis2D.Y))
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
                if (axes.HasFlag(Axis2D.X))
                {
                    normalizedKey.time = Mathf.InverseLerp(minKeyTime, maxKeyTime, key.time);
                }
                if (axes.HasFlag(Axis2D.Y))
                {
                    normalizedKey.value = Mathf.InverseLerp(minKeyValue, maxKeyValue, key.value);
                }
                result.AddKey(normalizedKey);
            }
            return result;
        }

        /// <summary>
        /// Calculates the derivative of the AnimationCurve at a given time using finite difference approximation.
        /// </summary>
        /// <param name="curve">The animation curve to evaluate.</param>
        /// <param name="time">The time (position at the X axis) at which to calculate the derivative.</param>
        /// <param name="eps">The small offset used for finite difference approximation.</param>
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