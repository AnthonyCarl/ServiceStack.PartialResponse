using System;
using System.Collections.Specialized;
using Rhino.Mocks;
using ServiceStack.ServiceHost;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class FieldsRetrieverTests
    {
        private IRequestContext GenerateRequestMockWithQueryString(
            string fieldsQueryStringName, params string[] fieldValues)
        {
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var mockHttpRequest = MockRepository.GenerateStub<IHttpRequest>();
            var nameValueCollection = MockRepository.GenerateStub<NameValueCollection>();
            string queryValue = string.Join(",", fieldValues);
            nameValueCollection.Expect(x => x.Get(fieldsQueryStringName)).Return(queryValue);
            mockHttpRequest.Expect(x => x.QueryString).Return(nameValueCollection);
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(mockHttpRequest);

            return reqContextMock;
        }

        [Fact]
        public void FieldsFromQueryString_NoFields_ReturnsEmptyString()
        {
            var mockRequestContext = MockRepository.GenerateStub<IRequestContext>();
            var mockHttpRequest = MockRepository.GenerateStub<IHttpRequest>();
            mockRequestContext.Expect(x => x.Get<IHttpRequest>()).Return(mockHttpRequest);

            Assert.Equal(
                string.Empty,
                FieldsRetriever.FieldsFromQueryString(
                    mockRequestContext, new DefaultPartialResponseConfig().FieldsQueryStringName));
        }

        [Fact]
        public void FieldsFromQueryString_OneField_OneFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            IRequestContext reqContextMock =
                GenerateRequestMockWithQueryString(partialReponseConfig.FieldsQueryStringName, IdField);

            Assert.Equal(
                IdField,
                FieldsRetriever.FieldsFromQueryString(reqContextMock, partialReponseConfig.FieldsQueryStringName));
        }

        [Fact]
        public void FieldsFromQueryString_TwoFields_TwoFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            const string NameField = "name";
            IRequestContext reqContextMock = GenerateRequestMockWithQueryString(
                partialReponseConfig.FieldsQueryStringName, IdField, NameField);

            Assert.Equal(
                string.Join(",", IdField, NameField),
                FieldsRetriever.FieldsFromQueryString(reqContextMock, partialReponseConfig.FieldsQueryStringName));
        }

        [Fact]
        public void FieldsFromQueryString_EmptyFields_ReturnsEmptyString()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            IRequestContext reqContextMock = GenerateRequestMockWithQueryString(
                partialReponseConfig.FieldsQueryStringName, string.Empty);

            Assert.Equal(
                string.Empty,
                FieldsRetriever.FieldsFromQueryString(reqContextMock, partialReponseConfig.FieldsQueryStringName));
        }


        [Fact]
        public void FieldsFromHeader_NoFields_ReturnsEmptyString()
        {
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            Assert.Equal(
                string.Empty,
                FieldsRetriever.FieldsFromHeader(
                    reqContextMock, new DefaultPartialResponseConfig().FieldsHeaderName));
        }

        [Fact]
        public void FieldsFromHeader_OneField_OneFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(IdField);

            Assert.Equal(
                IdField, FieldsRetriever.FieldsFromHeader(reqContextMock, partialReponseConfig.FieldsHeaderName));
        }

        [Fact]
        public void FieldsFromHeader_TwoFields_TwoFieldReturned()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            const string IdField = "id";
            const string NameField = "name";
            string joinedHeaderValues = string.Join(",", IdField, NameField);
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(joinedHeaderValues);

            Assert.Equal(
                joinedHeaderValues,
                FieldsRetriever.FieldsFromHeader(reqContextMock, partialReponseConfig.FieldsHeaderName));
        }

        [Fact]
        public void FieldsFromHeader_EmptyFields_ReturnsEmptyString()
        {
            var partialReponseConfig = new DefaultPartialResponseConfig();
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(string.Empty);

            Assert.Equal(
                string.Empty, FieldsRetriever.FieldsFromHeader(reqContextMock, partialReponseConfig.FieldsHeaderName));
        }

        [Fact]
        public void GetFields_NoFields_ReturnsEmptyString()
        {
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            Assert.Equal(string.Empty, FieldsRetriever.GetFields(reqContextMock, new DefaultPartialResponseConfig()));
        }

        [Fact]
        public void GetFields_HeaderThenQueryStringOnlyQueryStringSet_ReturnsOneField()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.HeaderThenQueryString);

            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            httpRequesttMock.Expect(x => x.QueryString)
                            .Return(new NameValueCollection {{partialReponseConfig.FieldsQueryStringName, IdField}});

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_HeaderThenQueryStringBothSetDifferent_ReturnsHeaderFieldOnly()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.HeaderThenQueryString);

            const string IdField = "id";
            const string NameField = "name";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            httpRequesttMock.Expect(x => x.QueryString)
                            .Return(new NameValueCollection {{partialReponseConfig.FieldsQueryStringName, IdField}});
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(NameField);

            Assert.Equal(
                NameField,
                FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringThenHeaderOnlyHeaderSet_ReturnsOneField()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringThenHeader);

            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(IdField);

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringThenHeaderBothSetDifferent_ReturnsQueryStringFieldOnly()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringThenHeader);

            const string IdField = "id";
            const string NameField = "name";

            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            httpRequesttMock.Expect(x => x.QueryString)
                            .Return(new NameValueCollection {{partialReponseConfig.FieldsQueryStringName, IdField}});
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(NameField);

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringOnlyOneFieldSet_RetrunsOneField()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.QueryStringOnly);

            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            httpRequesttMock.Expect(x => x.QueryString)
                            .Return(new NameValueCollection {{partialReponseConfig.FieldsQueryStringName, IdField}});

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_HeaderOnlyOneFieldSet_RetrunsOneField()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.HeaderOnly);

            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(IdField);

            Assert.Equal(
                IdField,
                FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringOnlyHeaderSet_RetrunsEmptyString()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.QueryStringOnly);

            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(IdField);

            Assert.Equal(string.Empty, FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_HeaderOnlyQueryStringSet_ReturnsEmptyString()
        {
            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig(FieldResolutionMethod.HeaderOnly);

            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            httpRequesttMock.Expect(x => x.QueryString)
                            .Return(new NameValueCollection {{partialReponseConfig.FieldsQueryStringName, IdField}});

            Assert.Equal(string.Empty, FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_UnknownEnum_ReturnsEmptyString()
        {
            Assert.False(Enum.IsDefined(typeof (FieldResolutionMethod), Int32.MaxValue));

            IPartialResponseConfig partialReponseConfig = GetMockResponseConfig((FieldResolutionMethod) Int32.MaxValue);
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();

            Assert.Equal(string.Empty, FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringAndHeaderBothSet_ReturnsAllFields()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringAndHeader);

            const string IdField = "id";
            const string NameField = "name";

            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            httpRequesttMock.Expect(x => x.QueryString)
                            .Return(new NameValueCollection {{partialReponseConfig.FieldsQueryStringName, IdField}});
            reqContextMock.Expect(x => x.GetHeader(partialReponseConfig.FieldsHeaderName)).Return(NameField);

            Assert.Equal(
                string.Join(FieldSelectorConstants.MultipleFieldSeparator.ToString(), NameField, IdField),
                FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
        }

        [Fact]
        public void GetFields_QueryStringAndHeaderOnlyQueryStringSet_ReturnsQueryStringFields()
        {
            IPartialResponseConfig partialReponseConfig =
                GetMockResponseConfig(FieldResolutionMethod.QueryStringAndHeader);

            const string IdField = "id";
            var reqContextMock = MockRepository.GenerateStub<IRequestContext>();
            var httpRequesttMock = MockRepository.GenerateStub<IHttpRequest>();
            reqContextMock.Expect(x => x.Get<IHttpRequest>()).Return(httpRequesttMock);
            httpRequesttMock.Expect(x => x.QueryString)
                            .Return(new NameValueCollection {{partialReponseConfig.FieldsQueryStringName, IdField}});

            Assert.Equal(IdField, FieldsRetriever.GetFields(reqContextMock, partialReponseConfig));
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