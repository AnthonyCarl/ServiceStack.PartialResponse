using System.Collections.Generic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    /// <summary>
    ///     Supports "application/json", "application/jsv, and"text/html" content type with fields passed in via the
    ///     header "x-fields" and query string "fields".
    /// </summary>
    public sealed class DefaultPartialResponseConfig : IPartialResponseConfig
    {
        /// <summary>
        /// Default partial Response Config.
        /// </summary>
        public DefaultPartialResponseConfig()
        {
            SupportedResponseContentType = new List<string> {"application/json", "text/html", "application/jsv"};
            FieldResolutionMethod = FieldResolutionMethod.QueryStringAndHeader;
            FieldsHeaderName = "x-fields";
            FieldsQueryStringName = "fields";
        }

        /// <summary>
        /// List of content types that are supported for partial responses.
        /// </summary>
        public List<string> SupportedResponseContentType { get; private set; }

        /// <summary>
        /// Field selectors are determined by combining the header and query string parameter.
        /// </summary>
        public FieldResolutionMethod FieldResolutionMethod { get; private set; }

        /// <summary>
        /// Name of the headers that contains the field selectors.
        /// </summary>
        public string FieldsHeaderName { get; private set; }

        /// <summary>
        /// Name of the query string parameter that contains the field selectors.
        /// </summary>
        public string FieldsQueryStringName { get; private set; }
    }
}