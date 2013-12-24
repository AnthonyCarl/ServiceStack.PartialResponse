using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void IsDynamic_Null_False()
        {
            Type nullType = null;
            Assert.False(nullType.IsDynamic());
        }

        [Fact]
        public void IsDynamic_Expando_True()
        {
            var type = typeof(ExpandoObject);
            Assert.True(type.IsDynamic());
        }

        [Fact]
        public void IsDynamic_DynamicObject_True()
        {
            var type = typeof(DynamicObject);
            Assert.True(type.IsDynamic());
        }

        [Fact]
        public void IsDynamic_String_False()
        {
            var type = typeof (string);
            Assert.False(type.IsDynamic());
        }
    }
}
