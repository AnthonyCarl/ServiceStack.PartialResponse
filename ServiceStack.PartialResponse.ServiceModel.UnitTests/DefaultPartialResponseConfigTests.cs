using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class DefaultPartialResponseConfigTests
    {
        [TestMethod]
        public void SupportedResponseContentType_DefaultConstructor_DefaultValues()
        {
            var partialResponseConfig = new DefaultPartialResponseConfig();
            Assert.AreEqual(FieldResolutionMethod.QueryStringAndHeader, partialResponseConfig.FieldResolutionMethod);
            Assert.AreEqual("fields", partialResponseConfig.FieldsQueryStringName);
            Assert.AreEqual("x-fields", partialResponseConfig.FieldsHeaderName);
            Assert.AreEqual(3, partialResponseConfig.SupportedResponseContentType.Count);
            Assert.IsTrue(partialResponseConfig.SupportedResponseContentType.Contains("application/json"));
            Assert.IsTrue(partialResponseConfig.SupportedResponseContentType.Contains("application/jsv"));
            Assert.IsTrue(partialResponseConfig.SupportedResponseContentType.Contains("text/html"));
        }
    }
}