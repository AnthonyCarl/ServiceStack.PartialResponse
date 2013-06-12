using System.Collections.Generic;
using Rhino.Mocks;
using Xunit;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    public class PartialResponseConfigExtensionsTests
    {
        [Fact]
        public void IsSupportedContentType_ContainsSupportedType_ReturnsTrue()
        {
            const string ContentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType).Return(new List<string> {ContentType, "text/html"});
            Assert.True(configMock.IsSupportedContentType(ContentType));
        }

        [Fact]
        public void IsSupportedContentType_NullList_ReturnsFalse()
        {
            const string contentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType).Return(null);
            Assert.False(configMock.IsSupportedContentType(contentType));
        }

        [Fact]
        public void IsSupportedContentType_EmptyList_ReturnsFalse()
        {
            const string ContentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType).Return(new List<string>());
            Assert.False(configMock.IsSupportedContentType(ContentType));
        }

        [Fact]
        public void IsSupportedContentType_ListWithoutType_ReturnsFalse()
        {
            const string ContentType = "application/json";
            var configMock = MockRepository.GenerateMock<IPartialResponseConfig>();
            configMock.Expect(x => x.SupportedResponseContentType)
                      .Return(new List<string> {"text/html", "application/xml"});
            Assert.False(configMock.IsSupportedContentType(ContentType));
        }
    }
}