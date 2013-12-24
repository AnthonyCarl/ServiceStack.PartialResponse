using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class DynamicPropertyValueGetterTests
    {
        [Fact]
        public void CreateDynamicObjectValueGetter_NullInstance_NullGetter()
        {
            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(null, "Property");
            Assert.Null(getter);
        }

        [Fact]
        public void CreateDynamicObjectValueGetter_NullProperty_NullGetter()
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;
            expando["key"] = "value";
            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(expando, null);

            Assert.Null(getter);
        }

        [Fact]
        public void CreateDynamicObjectValueGetter_NullInstanceNullProperty_NullGetter()
        {
            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(null, null);
            Assert.Null(getter);
        }

        [Fact]
        public void CreateDynamicObjectValueGetter_NonDynamicInstance_NullGetter()
        {
            var fakeName = new FakeName {First = "John", Last = "Doe"};
            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(fakeName, "First");
            Assert.Null(getter);
        }

        [Fact]
        public void CreateDynamicObjectValueGetter_PropertyCaseDoesNotMatch_ValidGetterCreated()
        {
            const string propertyName = "THisisMYProPERtyNamE";
            const string propertyValue = "Value";

            var expando = new ExpandoObject() as IDictionary<string, object>;
            expando[propertyName] = propertyValue;
            
            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(expando, propertyName.ToLower());

            Assert.Equal(propertyName, getter.PropertyName);
            Assert.Equal(propertyValue, getter.GetPropertyValue(expando));
        }

        [Fact]
        public void CreateDynamicObjectValueGetter_EmptyStringProperty_ValidGetterCreated()
        {
            const string propertyName = "";
            const string propertyValue = "Value";

            var expando = new ExpandoObject() as IDictionary<string, object>;
            expando[propertyName] = propertyValue;

            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(expando, propertyName.ToLower());

            Assert.Equal(propertyName, getter.PropertyName);
            Assert.Equal(propertyValue, getter.GetPropertyValue(expando));
        }

        [Fact]
        public void CreateDynamicObjectValueGetter_PropertyDoesNotExistOnInstance_NullGetterCreated()
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;
            expando["PropertyName"] = "SomeValue";

            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(expando, "DoesNotExist");

            Assert.Null(getter);
        }

        [Fact]
        public void GetPropertyValue_PropertyDoesNotExistOnInstance_RuntimeBinderException()
        {
            const string propertyName = "PropertyName";
            const string propertyValue = "Value";

            var expando = new ExpandoObject() as IDictionary<string, object>;
            expando[propertyName] = propertyValue;

            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(expando, propertyName);

            var otherExpando = new ExpandoObject() as IDictionary<string, object>;
            Assert.Throws<RuntimeBinderException>(
                () =>
                    getter.GetPropertyValue(otherExpando)
                );
        }

        [Fact]
        public void GetPropertyValue_NullInstance_RuntimeBinderException()
        {
            const string propertyName = "PropertyName";
            const string propertyValue = "Value";

            var expando = new ExpandoObject() as IDictionary<string, object>;
            expando[propertyName] = propertyValue;

            var getter = DynamicPropertyValueGetter.CreateDynamicObjectValueGetter(expando, propertyName);

            Assert.Throws<ArgumentNullException>(
                () =>
                    getter.GetPropertyValue(null)
                );
        }
    }
}
