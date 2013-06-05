using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class PartialResponsinatorTests
    {
        [TestMethod]
        public void GetPartialResponse_FlatObject_ResultContainsOnlySelectedFields()
        {
            const string JaneDoeHomepageUri = "http://janedoe.com/";
            const string DeeDoeFirstName = "Dee";
            const string DeeDoeLastName = "Doe";
            const decimal JaneDoeSalary = 100000000581.00m;

            var fakeDto =
                new FakeFlatDto
                {
                    HomePage = new FakeLink {Rel = "homepage", Uri = JaneDoeHomepageUri},
                    Name = new FakeName {First = DeeDoeFirstName, Last = DeeDoeLastName},
                    Salary = JaneDoeSalary
                };

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("Salary"),
                new FieldSelectorTreeNode("Name"),
                new FieldSelectorTreeNode("HomePage")
                {
                    Children = new List<FieldSelectorTreeNode>
                    {
                        new FieldSelectorTreeNode("Uri")
                    }
                }
            };


            var responsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic responsible = responsinator.GetPartialResponse(fakeDto);

            Assert.AreEqual(DeeDoeFirstName, responsible.Name.First);
            Assert.AreEqual(DeeDoeLastName, responsible.Name.Last);
            Assert.AreEqual(JaneDoeHomepageUri, responsible.HomePage.Uri);
            var homePage = responsible.HomePage as IDictionary<string, object>;
            Assert.IsFalse(homePage.ContainsKey("Rel"));
        }

        [TestMethod]
        public void GetPartialResponse_ComplexObjectWithNestedLists_ResultContainsOnlySelectedFields()
        {
            //I don't particularly like this test. I am open to ideas for improvement.
            const string JaneDoeHomepageUri = "http://janedoe.com/";
            const decimal JaneDoeSalary = 100000000581.00m;
            const string JaneDoeFirstName = "Jane";
            const string JaneDoeLastName = "Doe";
            const string DeeDoeFirstName = "Dee";
            const string DeeDoeLastName = "Doe";

            var fakDto = new FakeDto
            {
                Salary = JaneDoeSalary,
                Person =
                    new FakePerson
                    {
                        Name = new FakeName {First = JaneDoeFirstName, Last = JaneDoeLastName},
                        ChildrenNames =
                            new List<FakeName> {new FakeName {First = DeeDoeFirstName, Last = DeeDoeLastName}}
                    },
                Links =
                    new List<FakeLink>
                    {
                        new FakeLink
                        {
                            Rel = "homepage",
                            Uri = JaneDoeHomepageUri
                        }
                    }
            };

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("Salary"),
                new FieldSelectorTreeNode("Person")
                {
                    Children = new List<FieldSelectorTreeNode>
                    {
                        new FieldSelectorTreeNode("ChildrenNames")
                    }
                },
                new FieldSelectorTreeNode("Links")
                {
                    Children = new List<FieldSelectorTreeNode>
                    {
                        new FieldSelectorTreeNode("Uri")
                    }
                }
            };

            var responsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResponse = responsinator.GetPartialResponse(fakDto);

            Assert.AreEqual(JaneDoeSalary, partialResponse.Salary);

            Assert.AreEqual(1, partialResponse.Person.ChildrenNames.Count);
            Assert.AreEqual(DeeDoeFirstName, partialResponse.Person.ChildrenNames[0].First);
            Assert.AreEqual(DeeDoeLastName, partialResponse.Person.ChildrenNames[0].Last);

            var firstLink = partialResponse.Links[0] as IDictionary<string, object>;
            Assert.AreEqual(1, partialResponse.Links.Count);
            Assert.AreEqual(JaneDoeHomepageUri, firstLink["Uri"]);
            Assert.IsFalse(firstLink.ContainsKey("Rel"), "Rel should not be present on link.");

            var person = partialResponse.Person as IDictionary<string, object>;
            Assert.IsFalse(person.ContainsKey("Name"), "Name should not be present on Person");
        }

        [TestMethod]
        public void GetPartialResponse_RootIsListOfNames_ResultContainsOnlyFirstNames()
        {
            const string Person0FirstName = "Thor";
            const string Person1FirstName = "Jenkins";
            var myDto = new List<FakeName>
            {
                new FakeName {First = Person0FirstName, Last = string.Empty},
                new FakeName {First = Person1FirstName, Last = string.Empty}
            };

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("First"),
            };

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);

            Assert.AreEqual(2, partialResp.Count);
            Assert.AreEqual(Person0FirstName, partialResp[0].First);
            Assert.AreEqual(Person1FirstName, partialResp[1].First);
        }

        [TestMethod]
        public void GetPartialResponse_NullResponseOneEntryFieldSelectorList_ResultIsNull()
        {
            var responsinator =
                new PartialResponsinator(new List<FieldSelectorTreeNode> {new FieldSelectorTreeNode("Member")});
            Assert.IsNull(responsinator.GetPartialResponse(null));
        }

        [TestMethod]
        public void GetPartialResponse_NullFieldSelectorListResponseHasValue_ResultIsNull()
        {
            var responsinator = new PartialResponsinator(null);
            DateTimeOffset response = DateTimeOffset.UtcNow;
            Assert.AreEqual(response, responsinator.GetPartialResponse(response));
        }

        [TestMethod]
        public void GetPartialResponse_EmptyFieldSelectorListResponseHasValue_ResultIsNull()
        {
            var responsinator = new PartialResponsinator(new List<FieldSelectorTreeNode>());
            DateTimeOffset response = DateTimeOffset.UtcNow;
            Assert.AreEqual(response, responsinator.GetPartialResponse(response));
        }

        [TestMethod]
        public void GetPartialResponse_ListOfNulls_ReturnsEmptyList()
        {
            var myDto = new List<FakeName> {null, null, null};
            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("First"),
            };

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);

            Assert.AreEqual(0, partialResp.Count);
        }

        [TestMethod]
        public void GetPartialResponse_ListOfMixedWithNulls_ReturnsListWithoutNull()
        {
            const string Person0FirstName = "Thor";
            const string Person1FirstName = "Jenkins";
            var myDto = new List<FakeName>
            {
                new FakeName {First = Person0FirstName, Last = string.Empty},
                null,
                new FakeName {First = Person1FirstName, Last = string.Empty}
            };

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("First"),
            };

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);
            Assert.AreEqual(2, partialResp.Count);
            Assert.AreEqual(Person0FirstName, partialResp[0].First);
            Assert.AreEqual(Person1FirstName, partialResp[1].First);
        }

        [TestMethod]
        public void GetPartialResponse_InvalidFieldSelectionOnList_ReturnsEmptyList()
        {
            const string Person0FirstName = "Thor";
            const string Person1FirstName = "Jenkins";
            var myDto = new List<FakeName>
            {
                new FakeName {First = Person0FirstName, Last = string.Empty},
                new FakeName {First = Person1FirstName, Last = string.Empty}
            };

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("DoesNotExist"),
            };

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);

            Assert.AreEqual(0, partialResp.Count);
        }

        [TestMethod]
        public void GetPartialResponse_InvalidFieldSelectionOnFlatObj_ReturnsEmptyDynamic()
        {
            const string Person0FirstName = "Thor";
            var myDto = new FakeName {First = Person0FirstName, Last = string.Empty};

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("DoesNotExist"),
            };

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);

            Assert.IsNotNull(partialResp);

            var asDict = partialResp as IDictionary<string, object>;

            Assert.AreEqual(0, asDict.Count);
        }

        [TestMethod]
        public void GetPartialResponse_DtoHasNestedNulList_ReturnsEmptyDynamic()
        {
            var fakeDto = new FakeDto {Links = null};

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("Links"),
            };
            var partialResponsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResp = partialResponsinator.GetPartialResponse(fakeDto);

            var asDict = partialResp as IDictionary<string, object>;

            Assert.AreEqual(0, asDict.Count);
        }
    }
}