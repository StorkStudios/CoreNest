using System;

namespace StorkStudios.CoreNest
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public class FoldoutGroupAttribute : Attribute
    {
        public string Id { get; private set; }
        public string Header { get; private set; }

        public FoldoutGroupAttribute(string id, string header = null)
        {
            Id = id;
            Header = header ?? id;
        }
    }
}