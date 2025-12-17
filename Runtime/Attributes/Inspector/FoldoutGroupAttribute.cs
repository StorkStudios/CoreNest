using System;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Specifies that the associated field or method should be displayed within a foldout group in an inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public class FoldoutGroupAttribute : Attribute
    {
        public string Id { get; private set; }
        public string Header { get; private set; }

        /// <param name="id">Id string of the group. If <paramref name="header"/> is null this will be used as the group name.</param>
        /// <param name="header">Display name of the group shown in the header.</param>
        public FoldoutGroupAttribute(string id, string header = null)
        {
            Id = id;
            Header = header ?? id;
        }
    }
}