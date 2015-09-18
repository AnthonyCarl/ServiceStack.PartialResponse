using System.Collections.Generic;
using ServiceStack.Web;

namespace ServiceStack.PartialResponse.ServiceModel
{
    /// <summary>
    /// Takes a DTO and returns the appropriate partial response for the given configuration.
    /// </summary>
    public static class PartialResponseExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns a partial response of the given Dto if fields are specified in the request and the content
        ///         type is supported.
        ///     </para>
        ///     <para>
        ///         Uses <seealso cref="DefaultPartialResponseConfig" /> for partial response configuration.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">Dto Class</typeparam>
        /// <param name="request">Servicestack Request</param>
        /// <param name="dto">Dto to process to partial response.</param>
        /// <returns></returns>
        public static object ToPartialResponse<T>(this IRequest request, T dto)
            where T : class
        {
            return request.ToPartialResponse(dto, new DefaultPartialResponseConfig());
        }

        /// <summary>
        ///     <para>
        ///         Returns a partial response of the given Dto if fields are specified in the request and the content
        ///         type is supported.
        ///     </para>
        ///     <para>Configuration is provided by the caller.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="dto"></param>
        /// <param name="partialResponseConfig"></param>
        /// <returns></returns>
        public static object ToPartialResponse<T>(
            this IRequest request, T dto, IPartialResponseConfig partialResponseConfig)
            where T : class
        {
            if (dto == null)
            {
                return null;
            }

            string fields = FieldsRetriever.GetFields(request, partialResponseConfig);
            bool isSupportedContentType =
                partialResponseConfig.IsSupportedContentType(request.ResponseContentType);

            object processedResponse = dto;

            if (!string.IsNullOrWhiteSpace(fields) && isSupportedContentType)
            {
                List<FieldSelectorTreeNode> fieldSelectors = FieldSelectorParser.Expand(fields);
                var responsinator = new PartialResponsinator(fieldSelectors, new PropertyValueGetterFactory());
                processedResponse = responsinator.GetPartialResponse(dto);
            }

            return processedResponse;
        }
    }
}