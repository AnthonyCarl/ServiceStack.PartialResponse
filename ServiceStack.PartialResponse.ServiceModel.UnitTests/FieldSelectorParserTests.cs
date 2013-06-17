using System;
using System.Collections.Generic;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class FieldSelectorParserTests
    {
        [Fact]
        public void Expand_ComplexNestedFieldString_ReturnsParsedTree()
        {
            List<FieldSelectorTreeNode> tree =
                FieldSelectorParser.Expand(
                    "person(name(first,last),address(zip,street)),person/salary,link(url,description/short)");

            FieldSelectorTreeNode personNode = tree.Find(item => item.MemberName == "person");

            Assert.NotNull(personNode);
            Assert.Equal(3, personNode.Children.Count);
            Assert.NotNull(personNode.Children.Find(item => item.MemberName == "salary"));

            FieldSelectorTreeNode nameNode = personNode.Children.Find(item => item.MemberName == "name");

            Assert.NotNull(nameNode);
            Assert.Equal(2, nameNode.Children.Count);
            Assert.NotNull(nameNode.Children.Find(item => item.MemberName == "first"));
            Assert.NotNull(nameNode.Children.Find(item => item.MemberName == "last"));

            FieldSelectorTreeNode addressNode = personNode.Children.Find(item => item.MemberName == "address");

            Assert.NotNull(addressNode);
            Assert.Equal(2, addressNode.Children.Count);
            Assert.NotNull(addressNode.Children.Find(item => item.MemberName == "zip"));
            Assert.NotNull(addressNode.Children.Find(item => item.MemberName == "street"));

            FieldSelectorTreeNode linkNode = tree.Find(item => item.MemberName == "link");

            Assert.NotNull(linkNode);
            Assert.Equal(2, linkNode.Children.Count);
            Assert.NotNull(linkNode.Children.Find(item => item.MemberName == "url"));

            FieldSelectorTreeNode descriptionNode = linkNode.Children.Find(item => item.MemberName == "description");

            Assert.NotNull(descriptionNode);
            Assert.Equal(1, descriptionNode.Children.Count);
            Assert.NotNull(descriptionNode.Children.Find(item => item.MemberName == "short"));
        }

        [Fact]
        public void Expand_SelectSingleField_OneFieldSelectorReturned()
        {
            const string MyField = "myField";
            List<FieldSelectorTreeNode> tree = FieldSelectorParser.Expand(MyField);
            Assert.Equal(1, tree.Count);
            Assert.Equal(MyField, tree[0].MemberName);
            Assert.True(tree[0].Children.IsEmpty());
        }

        [Fact]
        public void Expand_SelectOneNestedField_TreeHasOneNestedSelector()
        {
            const string RootField = "contacttypes";
            const string SubField = "type";
            string nestedFieldSelector = RootField + FieldSelectorConstants.NestedFieldSelector + SubField;

            List<FieldSelectorTreeNode> tree = FieldSelectorParser.Expand(nestedFieldSelector);
            Assert.Equal(1, tree.Count);
            Assert.Equal(RootField, tree[0].MemberName);

            Assert.Equal(1, tree[0].Children.Count);
            Assert.Equal(SubField, tree[0].Children[0].MemberName);
            Assert.True(tree[0].Children[0].Children.IsEmpty());
        }

        [Fact]
        public void Expand_FieldSelectorBegineWithReservedToken_ThrowArgumentException()
        {
            bool exceptionCaught = false;
            try
            {
                FieldSelectorParser.Expand(FieldSelectorConstants.BeginSubSelectExpression.ToString());
            }
            catch (ArgumentException ex)
            {
                exceptionCaught = true;
                Assert.True(
                    ex.Message.Contains("A reserved token can not be the first character of the fields selector."));
            }

            Assert.True(exceptionCaught, "No Argument Exception caught!");
        }

        [Fact]
        public void Expand_EmptyFieldSelectorString_ReturnsEmptyList()
        {
            Assert.Equal(0, FieldSelectorParser.Expand(string.Empty).Count);
        }

        [Fact]
        public void Expand_NullFieldSelectorString_ReturnsEmptyList()
        {
            Assert.Equal(0, FieldSelectorParser.Expand(null).Count);
        }

        [Fact]
        public void Expand_WhitespaceFieldSelectorString_ReturnsEmptyList()
        {
            Assert.Equal(0, FieldSelectorParser.Expand("  \t  \n  \r").Count);
        }

        [Fact]
        public void Expand_NestedTokenPreceedByAnotherToken_ThrowArgumentException()
        {
            bool exceptionCaught = false;
            try
            {
                FieldSelectorParser.Expand(
                    "myField" + FieldSelectorConstants.BeginSubSelectExpression
                    + FieldSelectorConstants.NestedFieldSelector);
            }
            catch (ArgumentException ex)
            {
                exceptionCaught = true;
                Assert.True(ex.Message.Contains("Nested Field token"));
                Assert.True(ex.Message.Contains("can not be preceeded by another reserved token"));
            }

            Assert.True(exceptionCaught, "No Argument Exception caught!");
        }

        [Fact]
        public void Expand_SubSelectionTokenPreceedByAnotherToken_ThrowArgumentException()
        {
            bool exceptionCaught = false;
            try
            {
                FieldSelectorParser.Expand(
                    "myField" + FieldSelectorConstants.NestedFieldSelector
                    + FieldSelectorConstants.BeginSubSelectExpression);
            }
            catch (ArgumentException ex)
            {
                exceptionCaught = true;
                Assert.True(ex.Message.Contains("Begin Subselection token"));
                Assert.True(ex.Message.Contains("can not be preceeded by another reserved token"));
            }

            Assert.True(exceptionCaught, "No Argument Exception caught!");
        }
    }
}