using System.Collections.Generic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal static class CollectionExtensions
    {
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            //Comment
            return collection == null || collection.Count == 0;
        }
    }
}
