using System;
using UnityEngine;

namespace StorkStudios.CoreNest
{
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