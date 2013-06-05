using System.Collections.Generic;

namespace ServiceStack.Plugins.PartialResponse
{
    internal sealed class FieldSelectorTreeNode
    {
        public FieldSelectorTreeNode(string memberName)
        {
            MemberName = memberName;
            Children = new List<FieldSelectorTreeNode>();
        }

        public string MemberName { get; private set; }

        public List<FieldSelectorTreeNode> Children { get; set; }

        public FieldSelectorTreeNode GetOrAddChildNode(string memberName)
        {
            FieldSelectorTreeNode childNode = Children.Find(node => node.MemberName == memberName);

            if (childNode == null)
            {
                childNode = new FieldSelectorTreeNode(memberName);
                Children.Add(childNode);
            }

            return childNode;
        }
    }
}