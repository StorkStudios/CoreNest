using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Attribute that require implementation of the provided interface.
    /// Generic interfaces must implement the <see cref="IGenericInterface"/> interface.
    /// </summary>
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        // Interface type.
        public System.Type requiredType { get; private set; }
        /// <summary>
        /// Requiring implementation of the <see cref="T:RequireInterfaceAttribute"/> interface.
        /// </summary>
        /// <param name="type">Interface type.</param>
        public RequireInterfaceAttribute(System.Type type)
        {
            this.requiredType = type;
        }
    }
}