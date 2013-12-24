using System;
using System.Reflection;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal class PropertyInfoPropertyValueGetter : IPropertyValueGetter
    {
        private readonly PropertyInfo _propertyInfo;

        public static IPropertyValueGetter CreatePropertyInfoPropertyValueGetter(object instance, string propertyName)
        {
            if (instance == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var propertyInfo = instance.GetType().GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return propertyInfo == null ? null : new PropertyInfoPropertyValueGetter(propertyInfo);

        }
        
        private PropertyInfoPropertyValueGetter(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public object GetPropertyValue(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            return _propertyInfo.GetValue(instance, null);
        }

        public string PropertyName { get { return _propertyInfo.Name; } }
    }
}