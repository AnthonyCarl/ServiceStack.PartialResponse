using System.Dynamic;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal class PropertyValueGetterFactory : IPropertyValueGetterFactory
    {
        public IPropertyValueGetter CreatePropertyValueGetter(object instance, string propertyName)
        {
            if (instance is IDynamicMetaObjectProvider)
            {
                return DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(instance, propertyName);
            }
            return PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(instance, propertyName);
        }
    }
}