using System;
using System.Collections.Generic;
using System.Linq;

namespace Argh.NET.Helpers
{
    internal static class Subsets
    {
        public static IEnumerable<T> GetSubsetWithTypes<T>(this IEnumerable<T> source, IEnumerable<Type> allowedTypes)
        {
            var allowedTypesL = allowedTypes.ToArray();
            foreach (var elem in source)
            {
                if (allowedTypesL.Contains(elem.GetType()))
                {
                    yield return elem;
                }
            }
        }
    }
}
