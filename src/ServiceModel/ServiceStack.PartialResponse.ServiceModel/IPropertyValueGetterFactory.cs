namespace ServiceStack.PartialResponse.ServiceModel
{
    internal interface IPropertyValueGetterFactory
    {
        IPropertyValueGetter CreatePropertyValueGetter(object instance, string propertyName);
    }
}