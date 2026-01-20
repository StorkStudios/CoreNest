using UnityEngine;

namespace StorkStudios.CoreNest
{
    /// <summary>
    /// Creates a menu option in the inspector to make the referenced object a sub-asset of the parent asset. 
    /// The parent asset is considered to be the asset containing the object with the field with this attribute.
    /// Can be applied to any <see cref="Object"/> reference that can be a sub-asset, so GemeObject and Component references are excluded.
    /// </summary>
    public class SubAssetAttribute : PropertyAttribute
    {

    }
}