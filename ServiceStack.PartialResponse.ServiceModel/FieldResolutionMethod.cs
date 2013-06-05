namespace ServiceStack.Plugins.PartialResponse
{
    public enum FieldResolutionMethod
    {
        HeaderOnly,
        QueryStringOnly,
        HeaderThenQueryString,
        QueryStringThenHeader,
        QueryStringAndHeader
    }
}