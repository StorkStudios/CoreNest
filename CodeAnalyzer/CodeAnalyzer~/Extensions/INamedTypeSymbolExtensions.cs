using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorkStudios.CoreNest.CodeAnalyzer
{
    public static class INamedTypeSymbolExtensions
    {
        /// <summary>
        /// Returns the full name of the first base type that the given type derives from, or null if none of the specified base types are found in the inheritance hierarchy.
        /// </summary>
        /// <returns></returns>
        public static string DerivesFromAnyOf(this INamedTypeSymbol type, IEnumerable<string> baseTypeFullNames)
        {
            for (INamedTypeSymbol current = type.BaseType; current != null; current = current.BaseType)
            {
                if (baseTypeFullNames.Contains(current.ToDisplayString()))
                {
                    return current.ToDisplayString();
                }
            }

            return null;
        }
    }
}
