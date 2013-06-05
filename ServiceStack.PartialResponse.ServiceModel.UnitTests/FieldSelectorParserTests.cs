using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.Common;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class FieldSelectorParserTests
    {
        [TestMethod]
        public void Expand_ComplexNestedFieldString_ReturnsParsedTree()
        {
            List<FieldSelectorTreeNode> tree =
                FieldSelectorParser.Expand(
                    "person(name(first,last),address(zip,street)),person/salary,link(url,description/short)");

            FieldSelectorTreeNode personNode = tree.Find(item => item.MemberName == "person");

            Assert.IsNotNull(personNode);
            Assert.AreEqual(3, personNode.Children.Count);
            Assert.IsNotNull(personNode.Children.Find(item => item.MemberName == "salary"));

            FieldSelectorTreeNode nameNode = personNode.Children.Find(item => item.MemberName == "name");

            Assert.IsNotNull(nameNode);
            Assert.AreEqual(2, nameNode.Children.Count);
            Assert.IsNotNull(nameNode.Children.Find(item => item.MemberName == "first"));
            Assert.IsNotNull(nameNode.Children.Find(item => item.MemberName == "last"));

            FieldSelectorTreeNode addressNode = personNode.Children.Find(item => item.MemberName == "address");

            Assert.IsNotNull(addressNode);
            Assert.AreEqual(2, addressNode.Children.Count);
            Assert.IsNotNull(addressNode.Children.Find(item => item.MemberName == "zip"));
            Assert.IsNotNull(addressNode.Children.Find(item => item.MemberName == "street"));

            FieldSelectorTreeNode linkNode = tree.Find(item => item.MemberName == "link");

            Assert.IsNotNull(linkNode);
            Assert.AreEqual(2, linkNode.Children.Count);
            Assert.IsNotNull(linkNode.Children.Find(item => item.MemberName == "url"));

            FieldSelectorTreeNode descriptionNode = linkNode.Children.Find(item => item.MemberName == "description");

            Assert.IsNotNull(descriptionNode);
            Assert.AreEqual(1, descriptionNode.Children.Count);
            Assert.IsNotNull(descriptionNode.Children.Find(item => item.MemberName == "short"));
        }

        [TestMethod]
        public void Expand_SelectSingleField_OneFieldSelectorReturned()
        {
            const string MyField = "myField";
            List<FieldSelectorTreeNode> tree = FieldSelectorParser.Expand(MyField);
            Assert.AreEqual(1, tree.Count, "There should only be one field selector.");
            Assert.AreEqual(MyField, tree[0].MemberName);
            Assert.IsTrue(tree[0].Children.IsEmpty());
        }

        [TestMethod]
        public void Expand_SelectOneNestedField_TreeHasOneNestedSelector()
        {
            const string RootField = "contacttypes";
            const string SubField = "type";
            string nestedFieldSelector = RootField + FieldSelectorConstants.NestedFieldSelector + SubField;

            List<FieldSelectorTreeNode> tree = FieldSelectorParser.Expand(nestedFieldSelector);
            Assert.AreEqual(1, tree.Count, "There should only be one root field selector.");
            Assert.AreEqual(RootField, tree[0].MemberName);

            Assert.AreEqual(1, tree[0].Children.Count);
            Assert.AreEqual(SubField, tree[0].Children[0].MemberName);
            Assert.IsTrue(tree[0].Children[0].Children.IsEmpty());
        }

        [TestMethod]
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
                Assert.IsTrue(
                    ex.Message.Contains("A reserved token can not be the first character of the fields selector."));
            }

            Assert.IsTrue(exceptionCaught, "No Argument Exception caught!");
        }

        [TestMethod]
        public void Expand_EmptyFieldSelectorString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, FieldSelectorParser.Expand(string.Empty).Count);
        }

        [TestMethod]
        public void Expand_NullFieldSelectorString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, FieldSelectorParser.Expand(null).Count);
        }

        [TestMethod]
        public void Expand_WhitespaceFieldSelectorString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, FieldSelectorParser.Expand("  \t  \n  \r").Count);
        }

        [TestMethod]
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
                Assert.IsTrue(ex.Message.Contains("Nested Field token"));
                Assert.IsTrue(ex.Message.Contains("can not be preceeded by another reserved token"));
            }

            Assert.IsTrue(exceptionCaught, "No Argument Exception caught!");
        }

        [TestMethod]
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
                Assert.IsTrue(ex.Message.Contains("Begin Subselection token"));
                Assert.IsTrue(ex.Message.Contains("can not be preceeded by another reserved token"));
            }

            Assert.IsTrue(exceptionCaught, "No Argument Exception caught!");
        }
    }
}