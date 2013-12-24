using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IsGenericEnumerable_NullIEnumerable_False()
        {
            IEnumerable enumerable = null;
            Assert.False(enumerable.IsGenericEnumerable());
        }

        [Fact]
        public void IsGenericEnumerable_NullListOfStrings_False()
        {
            List<string> enumerable = null;
            Assert.False(enumerable.IsGenericEnumerable());
        }

        [Fact]
        public void IsGenericEnumerable_ExpandoObject_False()
        {
            IEnumerable expando = new ExpandoObject();
            Assert.False(expando.IsGenericEnumerable());
        }

        [Fact]
        public void IsGenericEnumerable_GenericListOfStrings_True()
        {
            var list = new List<string>();
            Assert.True(list.IsGenericEnumerable());
        }
    }
}
