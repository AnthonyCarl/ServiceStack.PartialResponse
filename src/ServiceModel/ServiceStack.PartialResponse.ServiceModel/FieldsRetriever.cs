using ServiceStack;
using ServiceStack.Web;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal static class FieldsRetriever
    {
        public static string GetFields(IRequest request, IPartialResponseConfig partialResponseConfig)
        {
            switch (partialResponseConfig.FieldResolutionMethod)
            {
                case FieldResolutionMethod.HeaderOnly:
                    return FieldsFromHeader(request, partialResponseConfig.FieldsHeaderName);
                case FieldResolutionMethod.QueryStringOnly:
                    return FieldsFromQueryString(request, partialResponseConfig.FieldsQueryStringName);
                case FieldResolutionMethod.HeaderThenQueryString:
                    {
                        string fields = FieldsFromHeader(request, partialResponseConfig.FieldsHeaderName);
                        return string.IsNullOrWhiteSpace(fields)
                                   ? FieldsFromQueryString(request, partialResponseConfig.FieldsQueryStringName)
                                   : fields;
                    }
                case FieldResolutionMethod.QueryStringThenHeader:
                    {
                        string fields = FieldsFromQueryString(request, partialResponseConfig.FieldsQueryStringName);
                        return string.IsNullOrWhiteSpace(fields)
                                   ? FieldsFromHeader(request, partialResponseConfig.FieldsHeaderName)
                                   : fields;
                    }
                case FieldResolutionMethod.QueryStringAndHeader:
                    {
                        string headerFields = FieldsFromHeader(request, partialResponseConfig.FieldsHeaderName);
                        string queryFields = FieldsFromQueryString(
                            request, partialResponseConfig.FieldsQueryStringName);
                        return string.IsNullOrWhiteSpace(headerFields)
                                   ? queryFields
                                   : string.Join(
                                       FieldSelectorConstants.MultipleFieldSeparator.ToString(), headerFields, queryFields);
                    }
                default:
                    return string.Empty;
            }
        }

        public static string FieldsFromQueryString(IRequest request, string fieldsQueryStringName)
        {
            if (request == null)
            {
                return string.Empty;
            }
            if (request.QueryString == null)
            {
                return string.Empty;
            }

            return request.QueryString.Get(fieldsQueryStringName);
        }

        public static string FieldsFromHeader(IRequest request, string fieldsHeaderName)
        {
            return request.Headers != null ? request.Headers[fieldsHeaderName] ?? string.Empty : string.Empty;
        }
    }
}