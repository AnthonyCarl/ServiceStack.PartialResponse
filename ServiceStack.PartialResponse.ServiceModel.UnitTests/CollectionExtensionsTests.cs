using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class CollectionExtensionsTests
    {
        [TestMethod]
        public void IsEmpty_NullList_ReturnsTrue()
        {
            ICollection<object> collection = null;
            Assert.IsTrue(collection.IsEmpty());
        }

        [TestMethod]
        public void IsEmpty_EmptyList_ReturnsTrue()
        {
            ICollection<object> collection = new List<object>();
            Assert.IsTrue(collection.IsEmpty());
        }

        [TestMethod]
        public void IsEmpty_OneItemList_ReturnsFalse()
        {
            ICollection<object> collection = new List<object> {new object()};
            Assert.IsFalse(collection.IsEmpty());
        }
    }
}