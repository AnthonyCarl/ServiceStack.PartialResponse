using ServiceStack.ServiceHost;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal static class FieldsRetriever
    {
        public static string GetFields(IRequestContext requestContext, IPartialResponseConfig partialResponseConfig)
        {
            switch (partialResponseConfig.FieldResolutionMethod)
            {
                case FieldResolutionMethod.HeaderOnly:
                    return FieldsFromHeader(requestContext, partialResponseConfig.FieldsHeaderName);
                case FieldResolutionMethod.QueryStringOnly:
                    return FieldsFromQueryString(requestContext, partialResponseConfig.FieldsQueryStringName);
                case FieldResolutionMethod.HeaderThenQueryString:
                {
                    string fields = FieldsFromHeader(requestContext, partialResponseConfig.FieldsHeaderName);
                    return string.IsNullOrWhiteSpace(fields)
                               ? FieldsFromQueryString(requestContext, partialResponseConfig.FieldsQueryStringName)
                               : fields;
                }
                case FieldResolutionMethod.QueryStringThenHeader:
                {
                    string fields = FieldsFromQueryString(requestContext, partialResponseConfig.FieldsQueryStringName);
                    return string.IsNullOrWhiteSpace(fields)
                               ? FieldsFromHeader(requestContext, partialResponseConfig.FieldsHeaderName)
                               : fields;
                }
                case FieldResolutionMethod.QueryStringAndHeader:
                {
                    string headerFields = FieldsFromHeader(requestContext, partialResponseConfig.FieldsHeaderName);
                    string queryFields = FieldsFromQueryString(
                        requestContext, partialResponseConfig.FieldsQueryStringName);
                    return string.IsNullOrWhiteSpace(headerFields)
                               ? queryFields
                               : string.Join(
                                   FieldSelectorConstants.MultipleFieldSeparator.ToString(), headerFields, queryFields);
                }
                default:
                    return string.Empty;
            }
        }

        public static string FieldsFromQueryString(IRequestContext requestContext, string fieldsQueryStringName)
        {
            var httpRequest = requestContext.Get<IHttpRequest>();

            if (httpRequest == null)
            {
                return string.Empty;
            }
            if (httpRequest.QueryString == null)
            {
                return string.Empty;
            }

            return httpRequest.QueryString.Get(fieldsQueryStringName);
        }

        public static string FieldsFromHeader(IRequestContext requestContext, string fieldsHeaderName)
        {
            return requestContext.GetHeader(fieldsHeaderName) ?? string.Empty;
        }
    }
}