using System.Collections.Generic;

namespace ServiceStack.Plugins.PartialResponse
{
    internal static class CollectionExtensions
    {
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}