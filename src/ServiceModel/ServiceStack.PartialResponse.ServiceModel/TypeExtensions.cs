using System;
using System.Dynamic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal static class TypeExtensions
    {
        public static bool IsDynamic(this Type type)
        {
            return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }
    }
}
