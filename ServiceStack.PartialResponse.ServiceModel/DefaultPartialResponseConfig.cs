using System.Collections.Generic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    public sealed class DefaultPartialResponseConfig : IPartialResponseConfig
    {
        /// <summary>
        ///     Supports "application/json", "application/jsv, and"text/html" content type with fields passed in via the
        ///     header "x-fields" and query string "fields".
        /// </summary>
        public DefaultPartialResponseConfig()
        {
            SupportedResponseContentType = new List<string> {"application/json", "text/html", "application/jsv"};
            FieldResolutionMethod = FieldResolutionMethod.QueryStringAndHeader;
            FieldsHeaderName = "x-fields";
            FieldsQueryStringName = "fields";
        }

        public List<string> SupportedResponseContentType { get; private set; }
        public FieldResolutionMethod FieldResolutionMethod { get; private set; }
        public string FieldsHeaderName { get; private set; }
        public string FieldsQueryStringName { get; private set; }
    }
}