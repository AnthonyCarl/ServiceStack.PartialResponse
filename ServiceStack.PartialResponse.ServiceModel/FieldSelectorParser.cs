using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceStack.PartialResponse.ServiceModel
{
    internal static class FieldSelectorParser
    {
        public static List<FieldSelectorTreeNode> Expand(string partialResponseFields)
        {
            if (string.IsNullOrWhiteSpace(partialResponseFields))
            {
                return new List<FieldSelectorTreeNode>();
            }

            if (FieldSelectorConstants.StartsWithReservedToken(partialResponseFields))
            {
                throw new ArgumentException(
                    "A reserved token can not be the first character of the fields selector.", "partialResponseFields");
            }

            var subSelectStack = new Stack<FieldSelectorTreeNode>();
            var nestedStack = new Stack<FieldSelectorTreeNode>();
            var currentMemberName = new StringBuilder();

            var parent = new FieldSelectorTreeNode(string.Empty);
            foreach (char c in partialResponseFields)
            {
                FieldSelectorTreeNode childNode;
                switch (c)
                {
                    case FieldSelectorConstants.NestedFieldSelector:
                        if (currentMemberName.Length == 0)
                        {
                            throw new ArgumentException(
                                string.Format(
                                    "Nested Field token '{0}' can not be preceeded by another reserved token.",
                                    FieldSelectorConstants.NestedFieldSelector),
                                "partialResponseFields");
                        }

                        childNode = parent.GetOrAddChildNode(currentMemberName.ToString());
                        currentMemberName = new StringBuilder();

                        nestedStack.Push(parent);
                        parent = childNode;
                        break;
                    case FieldSelectorConstants.MultipleFieldSeparator:
                        if (currentMemberName.Length != 0)
                        {
                            parent.GetOrAddChildNode(currentMemberName.ToString());
                            currentMemberName = new StringBuilder();
                        }
                        while (nestedStack.Count > 0)
                        {
                            parent = nestedStack.Pop();
                        }
                        break;
                    case FieldSelectorConstants.BeginSubSelectExpression:
                        if (currentMemberName.Length == 0)
                        {
                            throw new ArgumentException(
                                string.Format(
                                    "Begin Subselection token '{0}' can not be preceeded by another reserved token.",
                                    FieldSelectorConstants.BeginSubSelectExpression),
                                "partialResponseFields");
                        }

                        childNode = parent.GetOrAddChildNode(currentMemberName.ToString());
                        currentMemberName = new StringBuilder();

                        subSelectStack.Push(parent);
                        parent = childNode;

                        nestedStack = new Stack<FieldSelectorTreeNode>();
                        break;
                    case FieldSelectorConstants.EndSubSelectExpression:
                        if (currentMemberName.Length != 0)
                        {
                            parent.GetOrAddChildNode(currentMemberName.ToString());
                            currentMemberName = new StringBuilder();
                        }
                        parent = subSelectStack.Pop();
                        nestedStack = new Stack<FieldSelectorTreeNode>();
                        break;
                    default:
                        currentMemberName.Append(c);
                        break;
                }
            }

            if (currentMemberName.Length > 0)
            {
                parent.GetOrAddChildNode(currentMemberName.ToString());

                while (nestedStack.Count > 0)
                {
                    parent = nestedStack.Pop();
                }
            }

            return parent.Children;
        }
    }
}