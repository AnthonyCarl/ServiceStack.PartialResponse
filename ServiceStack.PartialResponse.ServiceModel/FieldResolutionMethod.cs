namespace ServiceStack.PartialResponse.ServiceModel
{
    /// <summary>
    /// Determines where field selectors come from (i.e. query string, header, etc).
    /// </summary>
    public enum FieldResolutionMethod
    {
        /// <summary>
        /// Only the header is use to determine field selectors.
        /// </summary>
        HeaderOnly,
        /// <summary>
        /// Only the query string parameter is used to determine field selectors.
        /// </summary>
        QueryStringOnly,
        /// <summary>
        /// Uses the header to determine field selectors. If the header is empty, the query string parameter is used.
        /// </summary>
        HeaderThenQueryString,
        /// <summary>
        /// Uses the query string parameter to determine field selectors. If the query string parameter is empty, the header is used.
        /// </summary>
        QueryStringThenHeader,
        /// <summary>
        /// Combines field selectors from the query string parameter and header.
        /// </summary>
        QueryStringAndHeader
    }
}