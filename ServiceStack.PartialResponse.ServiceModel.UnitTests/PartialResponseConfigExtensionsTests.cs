using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class PartialResponseConfigExtensionsTests
    {
        [TestMethod]
        public void IsSupportedContentType_ContainsSupportedType_ReturnsTrue()
        {
            const string ContentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType).Return(new List<string> {ContentType, "text/html"});
            Assert.IsTrue(configMock.IsSupportedContentType(ContentType));
        }

        [TestMethod]
        public void IsSupportedContentType_NullList_ReturnsFalse()
        {
            const string contentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType).Return(null);
            Assert.IsFalse(configMock.IsSupportedContentType(contentType));
        }

        [TestMethod]
        public void IsSupportedContentType_EmptyList_ReturnsFalse()
        {
            const string ContentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType).Return(new List<string>());
            Assert.IsFalse(configMock.IsSupportedContentType(ContentType));
        }

        [TestMethod]
        public void IsSupportedContentType_ListWithoutType_ReturnsFalse()
        {
            const string ContentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType)
                      .Return(new List<string> {"text/html", "application/xml"});
            Assert.IsFalse(configMock.IsSupportedContentType(ContentType));
        }
    }
}