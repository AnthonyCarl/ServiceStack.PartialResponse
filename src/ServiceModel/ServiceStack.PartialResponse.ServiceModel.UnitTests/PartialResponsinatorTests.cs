using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class PartialResponsinatorTests
    {
        [Fact]
        public void Constructor_NullFactory_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new PartialResponsinator(new List<FieldSelectorTreeNode>(), null)
                );
        }

        [Fact]
        public void GetPartialResponse_EmptyExpandoObject_ResultsIsEmptyExpandoObject()
        {
            var expando = new ExpandoObject();

            var responsinator =
                new PartialResponsinator(new List<FieldSelectorTreeNode> {new FieldSelectorTreeNode("SomeProperty")},
                    MockRepository.GenerateStub<IPropertyValueGetterFactory>());
            var partialResponse = responsinator.GetPartialResponse(expando) as IDictionary<string, object>;
            Assert.Empty(partialResponse);
        }

        [Fact]
        public void GetPartialResponse_BasicExpandoObject_ResultContainsSelectedFields()
        {
            const string ExpectedValue = "This is my expected value, there are many like it, but this one is mine";
            const string ExpectedKey = "Key";

            var expando = new ExpandoObject() as IDictionary<string, object>;
            expando.Add(ExpectedKey, ExpectedValue);
            expando.Add("AnotherKey", "AnotherValue");

            var getter = MockRepository.GenerateStub<IPropertyValueGetter>();
            getter.Stub(x => x.GetPropertyValue(null)).IgnoreArguments().Return(ExpectedValue);
            getter.Stub(x => x.PropertyName).Return(ExpectedKey);

            var factory = MockRepository.GenerateStub<IPropertyValueGetterFactory>();
            factory.Stub(x => x.CreatePropertyValueGetter(null, null)).IgnoreArguments().Return(getter);
            
            var responsinator =
                new PartialResponsinator(new List<FieldSelectorTreeNode> {new FieldSelectorTreeNode(ExpectedKey.ToLowerInvariant())}, factory);
            var partialResponse = responsinator.GetPartialResponse(expando) as IDictionary<string, object>;

           Assert.Equal(ExpectedValue, partialResponse[ExpectedKey]); 
        }

        [Fact]
        public void GetPartialResponse_FlatObject_ResultContainsOnlySelectedFields()
        {
            const string uriField = "Uri";
            const string salaryField = "Salary";
            const string homepageField = "HomePage";
            const string nameField = "Name";

            const string janeDoeHomepageUri = "http://janedoe.com/";
            const string deeDoeFirstName = "Dee";
            const string deeDoeLastName = "Doe";
            const decimal janeDoeSalary = 100000000581.00m;

            var homepage = new FakeLink {Rel = "homepage", Uri = janeDoeHomepageUri};
            var name = new FakeName {First = deeDoeFirstName, Last = deeDoeLastName};

            var propertyValues = new Dictionary<string, object>();
            propertyValues[uriField] = janeDoeHomepageUri;
            propertyValues[salaryField] = janeDoeSalary;
            propertyValues[homepageField] = homepage;
            propertyValues[nameField] = name;
     
            var fakeDto =
                new FakeFlatDto
                {
                    HomePage = homepage,
                    Name = name,
                    Salary = janeDoeSalary
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

            var factory = MockRepository.GenerateStub<IPropertyValueGetterFactory>();
            factory.Stub(x => x.CreatePropertyValueGetter(null, null)).IgnoreArguments().Return(null).WhenCalled(x =>
            {
                var getter = MockRepository.GenerateStub<IPropertyValueGetter>();
                getter.Stub(g => g.PropertyName).Return((string)x.Arguments[1]);
                getter.Stub(g => g.GetPropertyValue(null)).IgnoreArguments().Return(propertyValues[(string)x.Arguments[1]]);
                x.ReturnValue = getter;
            });

            var responsinator = new PartialResponsinator(partialFieldSelectors, factory);
            dynamic responsible = responsinator.GetPartialResponse(fakeDto);

            Assert.Equal(deeDoeFirstName, responsible.Name.First);
            Assert.Equal(deeDoeLastName, responsible.Name.Last);
            Assert.Equal(janeDoeHomepageUri, responsible.HomePage.Uri);
            var homePage = responsible.HomePage as IDictionary<string, object>;
            Assert.False(homePage.ContainsKey("Rel"));
        }

        [Fact]
        public void GetPartialResponse_ComplexObjectWithNestedLists_ResultContainsOnlySelectedFields()
        {
            //I don't particularly like this test. I am open to ideas for improvement.
            //This complex test may be a code smell that PartialResponsinator needs to be refactored and broken up.
            const string uriField = "Uri";
            const string salaryField = "Salary";
            const string firstField = "First";
            const string lastField = "Last";
            const string childrenNamesFiled = "ChildrenNames";
            const string personField = "Person";
            const string linksField = "Links";

            const string janeDoeHomepageUri = "http://janedoe.com/";
            const decimal janeDoeSalary = 100000000581.00m;
            const string janeDoeFirstName = "Jane";
            const string janeDoeLastName = "Doe";
            const string deeDoeFirstName = "Dee";
            const string deeDoeLastName = "Doe";

            var namesOfChildren = new List<FakeName> { new FakeName { First = deeDoeFirstName, Last = deeDoeLastName } };
            var person = new FakePerson
            {
                Name = new FakeName {First = janeDoeFirstName, Last = janeDoeLastName},
                ChildrenNames = namesOfChildren
            };

            var links = new List<FakeLink>
            {
                new FakeLink
                {
                    Rel = "homepage",
                    Uri = janeDoeHomepageUri
                }
            };

            var propertyValues = new Dictionary<string, object>();
            propertyValues[uriField] = janeDoeHomepageUri;
            propertyValues[salaryField] = janeDoeSalary;
            propertyValues[childrenNamesFiled] = namesOfChildren;
            propertyValues[personField] = person;
            propertyValues[firstField] = janeDoeFirstName;
            propertyValues[lastField] = janeDoeLastName;
            propertyValues[linksField] = links;

            var fakDto = new FakeDto
            {
                Salary = janeDoeSalary,
                Person = person,
                Links = links
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

            var factory = MockRepository.GenerateStub<IPropertyValueGetterFactory>();
            factory.Stub(x => x.CreatePropertyValueGetter(null, null)).IgnoreArguments().Return(null).WhenCalled(x =>
            {
                var getter = MockRepository.GenerateStub<IPropertyValueGetter>();
                getter.Stub(g => g.PropertyName).Return((string) x.Arguments[1]);
                getter.Stub(g => g.GetPropertyValue(null)).IgnoreArguments().Return(propertyValues[(string)x.Arguments[1]]);
                x.ReturnValue = getter;
            });

            var responsinator = new PartialResponsinator(partialFieldSelectors, factory);
            dynamic partialResponse = responsinator.GetPartialResponse(fakDto);

            Assert.Equal(janeDoeSalary, partialResponse.Salary);

            Assert.Equal(1, partialResponse.Person.ChildrenNames.Count);
            Assert.Equal(deeDoeFirstName, partialResponse.Person.ChildrenNames[0].First);
            Assert.Equal(deeDoeLastName, partialResponse.Person.ChildrenNames[0].Last);

            var firstLink = partialResponse.Links[0] as IDictionary<string, object>;
            Assert.Equal(1, partialResponse.Links.Count);
            Assert.Equal(janeDoeHomepageUri, firstLink["Uri"]);
            Assert.False(firstLink.ContainsKey("Rel"), "Rel should not be present on link.");

            var actualPerson = partialResponse.Person as IDictionary<string, object>;
            Assert.False(actualPerson.ContainsKey("Name"), "Name should not be present on Person");
        }

        [Fact]
        public void GetPartialResponse_RootIsListOfNames_ResultContainsOnlyFirstNames()
        {
            //This is similar to a test I detest
            const string person0FirstName = "Thor";
            const string person1FirstName = "Jenkins";
            var myDto = new List<FakeName>
            {
                new FakeName {First = person0FirstName, Last = string.Empty},
                new FakeName {First = person1FirstName, Last = string.Empty}
            };

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("First"),
            };

            var factory = MockRepository.GenerateStub<IPropertyValueGetterFactory>();
            factory.Stub(x => x.CreatePropertyValueGetter(null, null)).IgnoreArguments().Return(null).WhenCalled(x =>
            {
                var getter = MockRepository.GenerateStub<IPropertyValueGetter>();
                getter.Stub(g => g.PropertyName).Return((string)x.Arguments[1]);
                getter.Stub(g => g.GetPropertyValue(null))
                    .IgnoreArguments()
                    .Return(person0FirstName).Repeat.Once();
                getter.Stub(g => g.GetPropertyValue(null))
                    .IgnoreArguments()
                    .Return(person1FirstName).Repeat.Once();
                x.ReturnValue = getter;
            });

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors, factory);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);

            Assert.Equal(2, partialResp.Count);
            Assert.Equal(person0FirstName, partialResp[0].First);
            Assert.Equal(person1FirstName, partialResp[1].First);
        }

        [Fact]
        public void GetPartialResponse_NullResponseOneEntryFieldSelectorList_ResultNull()
        {
            var responsinator =
                new PartialResponsinator(new List<FieldSelectorTreeNode> { new FieldSelectorTreeNode("Member") }, MockRepository.GenerateStub<IPropertyValueGetterFactory>());
            Assert.Null(responsinator.GetPartialResponse(null));
        }

        [Fact]
        public void GetPartialResponse_NullFieldSelectorListResponseHasValue_ResultNull()
        {
            var responsinator = new PartialResponsinator(null, MockRepository.GenerateStub<IPropertyValueGetterFactory>());
            DateTimeOffset response = DateTimeOffset.UtcNow;
            Assert.Equal(response, responsinator.GetPartialResponse(response));
        }

        [Fact]
        public void GetPartialResponse_EmptyFieldSelectorListResponseHasValue_ResultNull()
        {
            var responsinator = new PartialResponsinator(new List<FieldSelectorTreeNode>(), MockRepository.GenerateStub<IPropertyValueGetterFactory>());
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

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors, MockRepository.GenerateStub<IPropertyValueGetterFactory>());
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);

            Assert.Equal(0, partialResp.Count);
        }

        [Fact]
        public void GetPartialResponse_ListOfMixedWithNulls_ReturnsListWithoutNull()
        {
            //Another test I detest
            const string person0FirstName = "Thor";
            const string person1FirstName = "Jenkins";

            var myDto = new List<FakeName>
            {
                new FakeName {First = person0FirstName, Last = string.Empty},
                null,
                new FakeName {First = person1FirstName, Last = string.Empty}
            };

            var partialFieldSelectors = new List<FieldSelectorTreeNode>
            {
                new FieldSelectorTreeNode("First"),
            };

            var factory = MockRepository.GenerateStub<IPropertyValueGetterFactory>();
            factory.Stub(x => x.CreatePropertyValueGetter(null, null)).IgnoreArguments().Return(null).WhenCalled(x =>
            {
                var getter = MockRepository.GenerateStub<IPropertyValueGetter>();
                getter.Stub(g => g.PropertyName).Return((string)x.Arguments[1]);
                getter.Stub(g => g.GetPropertyValue(null))
                    .IgnoreArguments()
                    .Return(person0FirstName).Repeat.Once();
                getter.Stub(g => g.GetPropertyValue(null))
                    .IgnoreArguments()
                    .Return(person1FirstName).Repeat.Once();
                x.ReturnValue = getter;
            });

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors, factory);
            dynamic partialResp = partialResponsinator.GetPartialResponse(myDto);
            Assert.Equal(2, partialResp.Count);
            Assert.Equal(person0FirstName, partialResp[0].First);
            Assert.Equal(person1FirstName, partialResp[1].First);
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

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors, MockRepository.GenerateStub<IPropertyValueGetterFactory>());
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

            var partialResponsinator = new PartialResponsinator(partialFieldSelectors, MockRepository.GenerateStub<IPropertyValueGetterFactory>());
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
            var partialResponsinator = new PartialResponsinator(partialFieldSelectors, MockRepository.GenerateStub<IPropertyValueGetterFactory>());
            dynamic partialResp = partialResponsinator.GetPartialResponse(fakeDto);

            var asDict = partialResp as IDictionary<string, object>;

            Assert.Equal(0, asDict.Count);
        }
    }
}