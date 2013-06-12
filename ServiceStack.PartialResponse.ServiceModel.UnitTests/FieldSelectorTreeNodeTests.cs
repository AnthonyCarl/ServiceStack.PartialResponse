using Xunit;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    public class FieldSelectorTreeNodeTests
    {
        [Fact]
        public void GetOrAddChildNode_NewChildNodeFromName_ContainsChildNode()
        {
            const string childNodeName = "ChildNode";

            var fieldSelectorNode = new FieldSelectorTreeNode(string.Empty);
            FieldSelectorTreeNode childNode = fieldSelectorNode.GetOrAddChildNode(childNodeName);

            Assert.True(fieldSelectorNode.Children.Contains(childNode));
            Assert.Equal(childNodeName, childNode.MemberName);
        }
    }
}