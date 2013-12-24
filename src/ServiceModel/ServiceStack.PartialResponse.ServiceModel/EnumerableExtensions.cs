using System.Collections;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal static class EnumerableExtensions
    {
        public static bool IsGenericEnumerable(this IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                return false;
            }

            return enumerable.GetType().IsGenericType;
        }
    }
}
