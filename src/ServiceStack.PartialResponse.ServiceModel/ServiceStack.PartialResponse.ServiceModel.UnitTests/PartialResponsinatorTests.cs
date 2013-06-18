using System;
using System.Collections.Generic;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class PartialResponsinatorTests
    {
        [Fact]
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

            Assert.Equal(DeeDoeFirstName, responsible.Name.First);
            Assert.Equal(DeeDoeLastName, responsible.Name.Last);
            Assert.Equal(JaneDoeHomepageUri, responsible.HomePage.Uri);
            var homePage = responsible.HomePage as IDictionary<string, object>;
            Assert.False(homePage.ContainsKey("Rel"));
        }

        [Fact]
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

            Assert.Equal(JaneDoeSalary, partialResponse.Salary);

            Assert.Equal(1, partialResponse.Person.ChildrenNames.Count);
            Assert.Equal(DeeDoeFirstName, partialResponse.Person.ChildrenNames[0].First);
            Assert.Equal(DeeDoeLastName, partialResponse.Person.ChildrenNames[0].Last);

            var firstLink = partialResponse.Links[0] as IDictionary<string, object>;
            Assert.Equal(1, partialResponse.Links.Count);
            Assert.Equal(JaneDoeHomepageUri, firstLink["Uri"]);
            Assert.False(firstLink.ContainsKey("Rel"), "Rel should not be present on link.");

            var person = partialResponse.Person as IDictionary<string, object>;
            Assert.False(person.ContainsKey("Name"), "Name should not be present on Person");
        }

        [Fact]
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

            Assert.Equal(2, partialResp.Count);
            Assert.Equal(Person0FirstName, partialResp[0].First);
            Assert.Equal(Person1FirstName, partialResp[1].First);
        }

        [Fact]
        public void GetPartialResponse_NullResponseOneEntryFieldSelectorList_ResultNull()
        {
            var responsinator =
                new PartialResponsinator(new List<FieldSelectorTreeNode> {new FieldSelectorTreeNode("Member")});
            Assert.Null(responsinator.GetPartialResponse(null));
        }

        [Fact]
        public void GetPartialResponse_NullFieldSelectorListResponseHasValue_ResultNull()
        {
            var responsinator = new PartialResponsinator(null);
            DateTimeOffset response = DateTimeOffset.UtcNow;
            Assert.Equal(response, responsinator.GetPartialResponse(response));
        }

        [Fact]
        public void GetPartialResponse_EmptyFieldSelectorListResponseHasValue_ResultNull()
        {
            var responsinator = new PartialResponsinator(new List<FieldSelectorTreeNode>());
            DateTimeOffset response = DateTimeOffset.UtcNow;
            Assert.Equal(response, responsinator.GetPartialResponse(response));
        }

        [Fact]
        public void GetPartialResponse_ListOfNulls_ReturnsEmptyList()
        {
            var myDto = new List<FakeName> {null, null, null};
            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("First"),
            };

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);

            Assert.Equal(0, partialResp.Count);
        }

        [Fact]
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
            Assert.Equal(2, partialResp.Count);
            Assert.Equal(Person0FirstName, partialResp[0].First);
            Assert.Equal(Person1FirstName, partialResp[1].First);
        }

        [Fact]
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

            Assert.Equal(0, partialResp.Count);
        }

        [Fact]
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

            Assert.NotNull(partialResp);

            var asDict = partialResp as IDictionary<string, object>;

            Assert.Equal(0, asDict.Count);
        }

        [Fact]
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

            Assert.Equal(0, asDict.Count);
        }
    }
}