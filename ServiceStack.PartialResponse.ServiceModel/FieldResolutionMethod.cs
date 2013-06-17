namespace ServiceStack.PartialResponse.ServiceModel
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