using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class DefaultPartialResponseConfigTests
    {
        [Fact]
        public void SupportedResponseContentType_DefaultConstructor_DefaultValues()
        {
            var partialResponseConfig = new DefaultPartialResponseConfig();
            Assert.Equal(FieldResolutionMethod.QueryStringAndHeader, partialResponseConfig.FieldResolutionMethod);
            Assert.Equal("fields", partialResponseConfig.FieldsQueryStringName);
            Assert.Equal("x-fields", partialResponseConfig.FieldsHeaderName);
            Assert.Equal(3, partialResponseConfig.SupportedResponseContentType.Count);
            Assert.True(partialResponseConfig.SupportedResponseContentType.Contains("application/json"));
            Assert.True(partialResponseConfig.SupportedResponseContentType.Contains("application/jsv"));
            Assert.True(partialResponseConfig.SupportedResponseContentType.Contains("text/html"));
        }
    }
}