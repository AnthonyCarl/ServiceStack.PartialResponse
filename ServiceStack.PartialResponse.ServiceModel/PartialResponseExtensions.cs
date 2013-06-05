using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace ServiceStack.Plugins.PartialResponse
{
    public static class PartialResponseExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns a partial response of the given Dto if fileds are specified in the request context and the content
        ///         type is supported.
        ///     </para>
        ///     <para>
        ///         Uses <seealso cref="DefaultPartialResponseConfig" /> for partial response configuration.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">Dto Class</typeparam>
        /// <param name="requestContext">Servicestack Request Context</param>
        /// <param name="dto">Dto to process to partial response.</param>
        /// <returns></returns>
        public static object ToPartialResponse<T>(this IRequestContext requestContext, T dto)
            where T : class
        {
            return requestContext.ToPartialResponse(dto, new DefaultPartialResponseConfig());
        }

        /// <summary>
        ///     <para>
        ///         Returns a partial response of the given Dto if fileds are specified in the request context and the content
        ///         type is supported.
        ///     </para>
        ///     <para>Configuration is provided by the caller.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <param name="dto"></param>
        /// <param name="partialResponseConfig"></param>
        /// <returns></returns>
        public static object ToPartialResponse<T>(
            this IRequestContext requestContext, T dto, IPartialResponseConfig partialResponseConfig)
            where T : class
        {
            if (dto == null)
            {
                return null;
            }

            string fields = FieldsRetriever.GetFields(requestContext, partialResponseConfig);
            bool isSupportedContentType =
                partialResponseConfig.IsSupportedContentType(requestContext.ResponseContentType);

            object processedResponse = dto;

            if (!string.IsNullOrWhiteSpace(fields) && isSupportedContentType)
            {
                List<FieldSelectorTreeNode> fieldSelectors = FieldSelectorParser.Expand(fields);
                var responsinator = new PartialResponsinator(fieldSelectors);
                processedResponse = responsinator.GetPartialResponse(dto);
            }

            return processedResponse;
        }
    }
}