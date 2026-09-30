using System;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Marks a class as a singleton which will generate additional code for this functionality.
    /// The class must be declared as <see langword="sealed"/> <see langword="partial"/> and inherit form <see cref="MonoBehaviour"/> or <see cref="ScriptableObject"/>.
    /// The singleton instance is lazily initialized meaning it is assigned in <see langword="Awake"/> for <see cref="MonoBehaviour"/> or when <c>Instance</c> is requested in <see cref="ScriptableObject"/>.
    /// </summary>
    /// <remarks><see cref="ScriptableObject"/> singletons must be created in <c>Asstes/Resources</c> folder or its subdirectories.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonAttribute : Attribute
    {
        public bool Persistent { get; }
        public bool AutoInit { get; }

        /// <param name="persistent">Can be applied to <see cref="MonoBehaviour"/>. Persistent singleton will be created if it doesn't exist and the <see cref="GameObject"/> containing it will be moved to <see cref="UnityEngine.Object.DontDestroyOnLoad"/>.</param>
        /// <param name="autoInit">Will create instance if it doesn't exist and automatically initialize it on scene load.</param>
        public SingletonAttribute(bool persistent = false, bool autoInit = false)
        {
            Persistent = persistent;
            AutoInit = autoInit;
        }
    }
}