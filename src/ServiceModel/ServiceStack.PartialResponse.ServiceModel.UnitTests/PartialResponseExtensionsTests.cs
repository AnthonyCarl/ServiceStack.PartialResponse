using System.Collections.Generic;
using Rhino.Mocks;
using ServiceStack.Web;
using Xunit;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class PartialResponseExtensionsTests
    {
        [Fact]
        public void ToPartialResponse_NoPartialFieldsSet_ReturnsSameObject()
        {
            var reqContextMock = MockRepository.GenerateMock<IRequest>();
            const decimal ExpectedSalary = 1234.56m;
            var fakeDto = new FakeDto {Salary = ExpectedSalary};
            object partialResponse = reqContextMock.ToPartialResponse(fakeDto);

            Assert.Same(fakeDto, partialResponse);
            Assert.Equal(ExpectedSalary, ((FakeDto) partialResponse).Salary);
        }

        [Fact]
        public void ToPartialResponse_NullDto_ReturnsNull()
        {
            FakeDto fakeDto = null;
            var reqContextMock = MockRepository.GenerateMock<IRequest>();

            Assert.Null(reqContextMock.ToPartialResponse(fakeDto));
        }

        [Fact]
        public void ToPartialResponse_UnsupportedType_ReturnsSameObject()
        {
            const decimal ExpectedSalary = 1234.56m;
            var fakeDto = new FakeDto {Salary = ExpectedSalary};
            var config = new DefaultPartialResponseConfig();
            var reqContextMock = MockRepository.GenerateMock<IRequest>();
            reqContextMock.Expect(x => x.ResponseContentType).Return(config.SupportedResponseContentType[0] + "garbage");
            reqContextMock.Expect(x => x.Headers.Get(config.FieldsHeaderName)).Return("id");
            object partialResponse = reqContextMock.ToPartialResponse(fakeDto, config);

            Assert.Same(fakeDto, partialResponse);
            Assert.Equal(ExpectedSalary, ((FakeDto) partialResponse).Salary);
        }

        [Fact]
        public void ToPartialResponse_FieldsSetAndSupportedType_ReturnspartialResponse()
        {
            const decimal ExpectedSalary = 1234.56m;
            var fakeDto = new FakeDto {Salary = ExpectedSalary, Person = new FakePerson()};
            var config = new DefaultPartialResponseConfig();
            var reqContextMock = MockRepository.GenerateMock<IRequest>();
            reqContextMock.Expect(x => x.ResponseContentType).Return(config.SupportedResponseContentType[0]);
            reqContextMock.Expect(x => x.Headers.Get(config.FieldsHeaderName)).Return("salary");
            dynamic partialResponse = reqContextMock.ToPartialResponse(fakeDto, config);

            Assert.False(((IDictionary<string, object>) partialResponse).ContainsKey("Person"));
            Assert.True(((IDictionary<string, object>) partialResponse).ContainsKey("Salary"));
            Assert.Equal(ExpectedSalary, partialResponse.Salary);
        }
    }
}