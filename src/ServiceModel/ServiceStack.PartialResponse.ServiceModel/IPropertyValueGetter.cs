namespace ServiceStack.PartialResponse.ServiceModel
{
    internal interface IPropertyValueGetter
    {
        object GetPropertyValue(object instance);
        string PropertyName { get; }
    }
}