using System;

namespace StorkStudios.CoreNest
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonAttribute : Attribute
    {
        public bool Persistent { get; }
        public bool AutoInit { get; }

        public SingletonAttribute(bool persistent = false, bool autoInit = false)
        {
            Persistent = persistent;
            AutoInit = autoInit;
        }
    }
}