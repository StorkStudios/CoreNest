using System;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Specifies that a <see cref="RangeBoundariesFloat"/> or <see cref="Vector2"/> field should be drawn with a min-max slider in the Unity Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public float Min => min;
        public float Max => max;

        private float min;
        private float max;

        public MinMaxRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}