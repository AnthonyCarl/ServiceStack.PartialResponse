using System.Collections.Generic;
using Xunit;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void IsEmpty_NullList_ReturnsTrue()
        {
            ICollection<object> collection = null;
            Assert.True(collection.IsEmpty());
        }

        [Fact]
        public void IsEmpty_EmptyList_ReturnsTrue()
        {
            ICollection<object> collection = new List<object>();
            Assert.True(collection.IsEmpty());
        }

        [Fact]
        public void IsEmpty_OneItemList_ReturnsFalse()
        {
            ICollection<object> collection = new List<object> {new object()};
            Assert.False(collection.IsEmpty());
        }
    }
}