using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class FieldSelectorTreeNodeTests
    {
        [TestMethod]
        public void GetOrAddChildNode_NewChildNodeFromName_ContainsChildNode()
        {
            const string childNodeName = "ChildNode";

            var fieldSelectorNode = new FieldSelectorTreeNode(string.Empty);
            FieldSelectorTreeNode childNode = fieldSelectorNode.GetOrAddChildNode(childNodeName);

            Assert.IsTrue(fieldSelectorNode.Children.Contains(childNode));
            Assert.AreEqual(childNodeName, childNode.MemberName);
        }
    }
}