using System.Collections.Generic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    public interface IPartialResponseConfig
    {
        List<string> SupportedResponseContentType { get; }
        FieldResolutionMethod FieldResolutionMethod { get; }
        string FieldsHeaderName { get; }
        string FieldsQueryStringName { get; }
    }
}