using System;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Draws an error icon when objectReferenceValue of this property is null
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : PropertyAttribute
    {

    }
}