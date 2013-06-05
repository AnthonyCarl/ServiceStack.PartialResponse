using System;
using System.Linq;
using ServiceStack.Common;

namespace ServiceStack.Plugins.PartialResponse
{
    internal static class PartialResponseConfigExtensions
    {
        public static bool IsSupportedContentType(this IPartialResponseConfig partialResponseConfig, string contentType)
        {
            return
                !partialResponseConfig.SupportedResponseContentType.IsEmpty()
                && partialResponseConfig.SupportedResponseContentType.Any(
                    s => s.Equals(contentType, StringComparison.OrdinalIgnoreCase));
        }
    }
}