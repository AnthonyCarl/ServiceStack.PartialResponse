using System;
using Rhino.Mocks;
using ServiceStack.Web;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class FieldsRetrieverTests
    {
        private void StubHeaders(IRequest request, string field, params string[] values)
        {
            StubHeaders(request, new Tuple<string, string[]>(field, values));
        }

        private void StubHeaders(IRequest request, params Tuple<string, string[]>[] headers)
        {
            var nameValueCollection = MockRepository.GenerateStub<INameValueCollection>();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    string headerValue = string.Join(FieldSelectorConstants.MultipleFieldSeparator.ToString(), header.Item2);
                    nameValueCollection.Expect(x => x.Get(header.Item1)).Return(headerValue);
                }
            }
            request.Expect(x => x.Headers).Return(nameValueCollection);
        }

        private void StubQueryString(IRequest request, string field, params string[] values)
        {
            StubQueryString(request, new Tuple<string, string[]>(field, values));
        }

        private void StubQueryString(IRequest request, params Tuple<string, string[]>[] queryString)
        {
            var nameValueCollection = MockRepository.GenerateStub<INameValueCollection>();
            if (queryString != null)
            {
                foreach (var queryParam in queryString)
                {
                    string queryValue = string.Join(FieldSelectorConstants.MultipleFieldSeparator.ToString(), queryParam.Item2);
                    nameValueCollection.Expect(x => x.Get(queryParam.Item1)).Return(queryValue);
                }
            }
            request.Expect(x => x.QueryString).Return(nameValueCollection);
        }

        [Fact]
        public void FieldsFromQueryString_NoFields_ReturnsEmptyString()
        {
            var request = MockRepository.GenerateStub<IRequest>();

            Assert.Equal(
                string.Empty,
                FieldsRetriever.FieldsFromQueryString(
                    request, new DefaultPartialResponseConfig().FieldsQueryStringName));
        }

        [Fact]
        public void FieldsFromQueryString_OneField_OneFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);

            Assert.Equal(
                IdField,
                FieldsRetriever.FieldsFromQueryString(request, partialReponseConfig.FieldsQueryStringName));
        }

        [Fact]
        public void FieldsFromQueryString_TwoFields_TwoFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            const string NameField = "name";
            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField, NameField);

            Assert.Equal(
                string.Join(FieldSelectorConstants.MultipleFieldSeparator.ToString(), IdField, NameField),
                FieldsRetriever.FieldsFromQueryString(request, partialReponseConfig.FieldsQueryStringName));
        }

        [Fact]
        public void FieldsFromQueryString_EmptyFields_ReturnsEmptyString()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, string.Empty);

            Assert.Equal(
                string.Empty,
                FieldsRetriever.FieldsFromQueryString(request, partialReponseConfig.FieldsQueryStringName));
        }


        [Fact]
        public void FieldsFromHeader_NoFields_ReturnsEmptyString()
        {
            var request = MockRepository.GenerateStub<IRequest>();
            StubHeaders(request);
            
            Assert.Equal(
                string.Empty,
                FieldsRetriever.FieldsFromHeader(
                    request, new DefaultPartialResponseConfig().FieldsHeaderName));
        }

        [Fact]
        public void FieldsFromHeader_OneField_OneFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, IdField);

            Assert.Equal(
                IdField, FieldsRetriever.FieldsFromHeader(request, partialReponseConfig.FieldsHeaderName));
        }

        [Fact]
        public void FieldsFromHeader_TwoFields_TwoFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            const string NameField = "name";
            var request = MockRepository.GenerateStub<IRequest>();
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, IdField, NameField);

            Assert.Equal(
                string.Join(FieldSelectorConstants.MultipleFieldSeparator.ToString(), IdField, NameField),
                FieldsRetriever.FieldsFromHeader(request, partialReponseConfig.FieldsHeaderName));
        }

        [Fact]
        public void FieldsFromHeader_EmptyFields_ReturnsEmptyString()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            var request = MockRepository.GenerateStub<IRequest>();
            request.Expect(x => x.Headers.Get(partialReponseConfig.FieldsHeaderName)).Return(string.Empty);

            Assert.Equal(
                string.Empty, FieldsRetriever.FieldsFromHeader(request, partialReponseConfig.FieldsHeaderName));
        }

        [Fact]
        public void GetFields_NoFields_ReturnsEmptyString()
        {
            var request = MockRepository.GenerateStub<IRequest>();
            Assert.Equal(string.Empty, FieldsRetriever.GetFields(request, new DefaultPartialResponseConfig()));
        }

        [Fact]
        public void GetFields_HeaderThenQueryStringOnlyQueryStringSet_ReturnsOneField()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.HeaderThenQueryString);

            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_HeaderThenQueryStringBothSetDifferent_ReturnsHeaderFieldOnly()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.HeaderThenQueryString);

            const string IdField = "id";
            const string NameField = "name";
            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, NameField);

            Assert.Equal(
                NameField,
                FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringThenHeaderOnlyHeaderSet_ReturnsOneField()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringThenHeader);

            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, IdField);

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringThenHeaderBothSetDifferent_ReturnsQueryStringFieldOnly()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringThenHeader);

            const string IdField = "id";
            const string NameField = "name";

            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, NameField);

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringOnlyOneFieldSet_RetrunsOneField()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.QueryStringOnly);

            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);
            
            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_HeaderOnlyOneFieldSet_RetrunsOneField()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.HeaderOnly);

            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, IdField);

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringOnlyHeaderSet_RetrunsEmptyString()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.QueryStringOnly);

            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, IdField);
            
            Assert.Equal(string.Empty, FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_HeaderOnlyQueryStringSet_ReturnsEmptyString()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.HeaderOnly);

            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);

            Assert.Equal(string.Empty, FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_UnknownEnum_ReturnsEmptyString()
        {
            Assert.False(Enum.IsDefined(typeof (FieldResolutionMethod), Int32.MaxValue));

            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig((FieldResolutionMethod) Int32.MaxValue);
            var request = MockRepository.GenerateStub<IRequest>();

            Assert.Equal(string.Empty, FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringAndHeaderBothSet_ReturnsAllFields()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringAndHeader);

            const string IdField = "id";
            const string NameField = "name";

            var request = MockRepository.GenerateStub<IRequest>();
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);
            StubHeaders(request, partialReponseConfig.FieldsHeaderName, NameField);

            Assert.Equal(
                string.Join(FieldSelectorConstants.MultipleFieldSeparator.ToString(), NameField, IdField),
                FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringAndHeaderOnlyQueryStringSet_ReturnsQueryStringFields()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringAndHeader);

            const string IdField = "id";
            var request = MockRepository.GenerateStub<IRequest>();
            StubHeaders(request);
            StubQueryString(request, partialReponseConfig.FieldsQueryStringName, IdField);

            Assert.Equal(IdField, FieldsRetriever.GetFields(request, partialReponseConfig));
        }

        private IPartialResponseConfig GetMockResponseConfig(FieldResolutionMethod resolutionMethod)
        {
            var partialReponseConfig = MockRepository.GenerateMock<IPartialResponseConfig>();
            partialReponseConfig.Expect(x => x.FieldResolutionMethod)
                                .Return(resolutionMethod);
            partialReponseConfig.Expect(x => x.FieldsQueryStringName).Return("fields");
            partialReponseConfig.Expect(x => x.FieldsHeaderName).Return("x-fields");

            return partialReponseConfig;
        }
    }
}