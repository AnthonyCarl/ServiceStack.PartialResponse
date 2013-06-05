using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ServiceStack.ServiceHost;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class PartialResponseExtensionsTests
    {
        [TestMethod]
        public void ToPartialResponse_NoPartialFieldsSet_ReturnsSameObject()
        {
            var reqContextMock = MockRepository.GenerateMock<IRequestContext>();
            const decimal ExpectedSalary = 1234.56m;
            var fakeDto = new FakeDto {Salary = ExpectedSalary};
            object partialResponse = reqContextMock.ToPartialResponse(fakeDto);

            Assert.AreSame(fakeDto, partialResponse);
            Assert.AreEqual(ExpectedSalary, ((FakeDto) partialResponse).Salary);
        }

        [TestMethod]
        public void ToPartialResponse_NullDto_ReturnsNull()
        {
            FakeDto fakeDto = null;
            var reqContextMock = MockRepository.GenerateMock<IRequestContext>();

            Assert.IsNull(reqContextMock.ToPartialResponse(fakeDto));
        }

        [TestMethod]
        public void ToPartialResponse_UnsupportedType_ReturnsSameObject()
        {
            const decimal ExpectedSalary = 1234.56m;
            var fakeDto = new FakeDto {Salary = ExpectedSalary};
            var config = new DefaultPartialResponseConfig();
            var reqContextMock = MockRepository.GenerateMock<IRequestContext>();
            reqContextMock.Expect(x => x.ResponseContentType).Return(config.SupportedResponseContentType[0] + "garbage");
            reqContextMock.Expect(x => x.GetHeader(config.FieldsHeaderName)).Return("id");
            object partialResponse = reqContextMock.ToPartialResponse(fakeDto, config);

            Assert.AreSame(fakeDto, partialResponse);
            Assert.AreEqual(ExpectedSalary, ((FakeDto) partialResponse).Salary);
        }

        [TestMethod]
        public void ToPartialResponse_FieldsSetAndSupportedType_ReturnspartialResponse()
        {
            const decimal ExpectedSalary = 1234.56m;
            var fakeDto = new FakeDto {Salary = ExpectedSalary, Person = new FakePerson()};
            var config = new DefaultPartialResponseConfig();
            var reqContextMock = MockRepository.GenerateMock<IRequestContext>();
            reqContextMock.Expect(x => x.ResponseContentType).Return(config.SupportedResponseContentType[0]);
            reqContextMock.Expect(x => x.GetHeader(config.FieldsHeaderName)).Return("salary");
            dynamic partialResponse = reqContextMock.ToPartialResponse(fakeDto, config);

            Assert.IsFalse(((IDictionary<string, object>) partialResponse).ContainsKey("Person"));
            Assert.IsTrue(((IDictionary<string, object>) partialResponse).ContainsKey("Salary"));
            Assert.AreEqual(ExpectedSalary, partialResponse.Salary);
        }
    }
}