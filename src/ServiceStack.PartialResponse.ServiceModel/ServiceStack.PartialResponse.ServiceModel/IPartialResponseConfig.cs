using System.Collections.Generic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    /// <summary>
    /// Implement this interface to provide custom configuration for partial responses.
    /// </summary>
    public interface IPartialResponseConfig
    {
        /// <summary>
        /// Lists custom supported content types.
        /// </summary>
        List<string> SupportedResponseContentType { get; }

        /// <summary>
        /// Specifies field resolution method.
        /// </summary>
        FieldResolutionMethod FieldResolutionMethod { get; }

        /// <summary>
        /// The name of the custom header for field selectors.
        /// </summary>
        string FieldsHeaderName { get; }

        /// <summary>
        /// The query string parameter name for the field selectors.
        /// </summary>
        string FieldsQueryStringName { get; }
    }
}