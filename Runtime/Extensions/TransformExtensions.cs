using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Interpolates transform's position, localScale and rotation using Unity's built-in Lerp methods.
        /// <see cref="Vector3.Lerp"/>
        /// <see cref="Quaternion.Lerp"/>
        /// </summary>
        /// <seealso cref="LerpUnclamped"/>
        public static void Lerp(this Transform transform, Transform a, Transform b, float t)
        {
            transform.position = Vector3.Lerp(a.position, b.position, t);
            transform.localScale = Vector3.Lerp(a.localScale, b.localScale, t);
            transform.rotation = Quaternion.Lerp(a.rotation, b.rotation, t);
        }

        /// <summary>
        /// Interpolates transform's position, localScale and rotation using Unity's built-in LerpUnclamped methods, allowing extrapolation beyond the end points.
        /// <see cref="Vector3.LerpUnclamped"/>
        /// <see cref="Quaternion.LerpUnclamped"/>
        /// </summary>
        /// <seealso cref="Lerp"/>
        public static void LerpUnclamped(this Transform transform, Transform a, Transform b, float t)
        {
            transform.position = Vector3.LerpUnclamped(a.position, b.position, t);
            transform.localScale = Vector3.LerpUnclamped(a.localScale, b.localScale, t);
            transform.rotation = Quaternion.LerpUnclamped(a.rotation, b.rotation, t);
        }
    }
}