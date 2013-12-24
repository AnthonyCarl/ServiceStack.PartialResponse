using System;
using System.Collections.Generic;
using ImpromptuInterface;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal class DynamicPropertyValueGetter : IPropertyValueGetter
    {
        public static IPropertyValueGetter CreateDynamicObjectValueGetter(dynamic instance, string propertyName)
        {
            List<string> memberNames = Impromptu.GetMemberNames(instance, true);
            string matchedName = memberNames.Find(
                s =>
                    s.Equals(propertyName,
                        StringComparison.OrdinalIgnoreCase));
            return matchedName == null ? null : new DynamicPropertyValueGetter(matchedName);
        }

        private DynamicPropertyValueGetter(string propertyName)
        {
            PropertyName = propertyName;
        }

        public object GetPropertyValue(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            return Impromptu.InvokeGet(instance, PropertyName);
        }

        public string PropertyName { get; private set; }
    }
}