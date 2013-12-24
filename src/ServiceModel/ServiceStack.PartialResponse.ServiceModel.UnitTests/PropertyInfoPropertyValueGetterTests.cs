using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class PropertyInfoPropertyValueGetterTests
    {
        [Fact]
        public void CreatePropertyInfoPropertyValueGetter_NullInstance_NullGetter()
        {
            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(null, "Property");
            Assert.Null(getter);
        }

        [Fact]
        public void CreatePropertyInfoPropertyValueGetter_NullProperty_NullGetter()
        {
            var fakeName = new FakeName {First = "Firstie", Last = "Lastly"};
            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(fakeName, null);

            Assert.Null(getter);
        }

        [Fact]
        public void CreatePropertyInfoPropertyValueGetter_NullInstanceNullProperty_NullGetter()
        {
            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(null, null);
            Assert.Null(getter);
        }

        [Fact]
        public void CreatePropertyInfoPropertyValueGetter_NonDynamicInstance_ValidGetterCreated()
        {
            const string firstProperty = "First";
            const string firstName = "John";

            var fakeName = new FakeName { First = firstName, Last = "Doe" };
            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(fakeName, firstProperty);

            Assert.Equal(firstProperty, getter.PropertyName);
            Assert.Equal(firstName, getter.GetPropertyValue(fakeName));
        }

        [Fact]
        public void CreatePropertyInfoPropertyValueGetter_PropertyCaseDoesNotMatch_ValidGetterCreated()
        {
            const string firstProperty = "fIRsT";
            const string firstName = "John";

            var fakeName = new FakeName { First = firstName, Last = "Doe" };

            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(fakeName, firstProperty);

            Assert.Equal("First", getter.PropertyName);
            Assert.Equal(firstName, getter.GetPropertyValue(fakeName));
        }

        [Fact]
        public void CreatePropertyInfoPropertyValueGetter_EmptyStringProperty_NullGetter()
        {
            const string propertyName = "";

            var fakeName = new FakeName {First = "Firstie", Last = "Lastly"};

            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(fakeName, propertyName.ToLower());

            Assert.Null(getter);
        }

        [Fact]
        public void CreatePropertyInfoPropertyValueGetter_PropertyDoesNotExistOnInstnace_NullGetterCreated()
        {
            var fakeName = new FakeName {First = "Thor", Last = "Jenkins"};
            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(fakeName, "DoesNotExist");

            Assert.Null(getter);
        }

        [Fact]
        public void GetPropertyValue_PropertyDoesNotExistOnInstance_TargetException()
        {
            const string propertyName = "First";
            
            var fakeName = new FakeName { First = "Thor", Last = "Jenkins" };

            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(fakeName, propertyName);

            var fakeLink = new FakeLink {Rel = "self", Uri = "http://localhost/"};
            Assert.Throws<TargetException>(
                () =>
                    getter.GetPropertyValue(fakeLink)
                );
        }

        [Fact]
        public void GetPropertyValue_NullInstance_RuntimeBinderException()
        {
            const string propertyName = "First";

            var fakeName = new FakeName {First = "Hank", Last = "The Tank"};

            var getter = PropertyInfoPropertyValueGetter.CreatePropertyInfoPropertyValueGetter(fakeName, propertyName);

            Assert.Throws<ArgumentNullException>(
                () =>
                    getter.GetPropertyValue(null)
                );
        }

    }
}
