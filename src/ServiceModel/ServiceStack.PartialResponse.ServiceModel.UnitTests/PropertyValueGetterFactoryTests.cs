using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class PropertyValueGetterFactoryTests
    {
        [Fact]
        public void CreatePropertyValueGetter_NullInstance_NullGetter()
        {
            var factory = new PropertyValueGetterFactory();
            var getter = factory.CreatePropertyValueGetter(null, "property");
            Assert.Null(getter);
        }

        [Fact]
        public void CreatePropertyValueGetter_DynamicObject_DynamicPropertyGetter()
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;
            const string propertyName = "Property";
            expando[propertyName] = "value";
            var factory = new PropertyValueGetterFactory();
            var getter = factory.CreatePropertyValueGetter(expando, propertyName);

            Assert.True(getter is DynamicPropertyValueGetter);
        }

        [Fact]
        public void CreatePropertyValueGetter_NonDynamic_PropertyInfoPropertyGetter()
        {
            var fakeName = new FakeName {First = "Joe", Last = "Dirt"};
            var factory = new PropertyValueGetterFactory();
            var getter = factory.CreatePropertyValueGetter(fakeName, "First");

            Assert.True(getter is PropertyInfoPropertyValueGetter);

        }
    }
}
