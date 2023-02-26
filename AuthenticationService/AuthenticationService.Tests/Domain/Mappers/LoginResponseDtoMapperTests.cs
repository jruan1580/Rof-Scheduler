using AuthenticationService.Domain.Mappers;
using AuthenticationService.DTO.Accessors;
using NUnit.Framework;

namespace AuthenticationService.Tests.Domain.Mappers
{
    [TestFixture]
    public class LoginResponseDtoMapperTests
    {
        [Test]
        public void TestMapEmployeeLoginResponseToBasicUserInfo()
        {
            var employeeLoginResponse = new EmployeeLoginResponse()
            {
                FirstName = "employeeFirstName",
                Id = 1,
                Role = "Employee"
            };

            var employeeInfo = LoginResponseDtoMapper.MapEmployeeLoginResponseToBasicUserInfo(employeeLoginResponse);

            Assert.IsNotNull(employeeInfo);
            Assert.AreEqual(employeeInfo.FirstName, employeeLoginResponse.FirstName);
            Assert.AreEqual(employeeInfo.Id, employeeLoginResponse.Id);
            Assert.AreEqual(employeeInfo.Role, employeeLoginResponse.Role);
        }

        [Test]
        public void TestMapEmployeeLoginResponseToBasicUserInfoReturnsNull()
        {
            var employeeInfo = LoginResponseDtoMapper.MapEmployeeLoginResponseToBasicUserInfo(null);

            Assert.IsNull(employeeInfo);
        }

        [Test]
        public void TestMapClientLoginResponseToBasicUserInfo()
        {
            var loginResponse = new LoginResponse()
            {
                Id = 1,
                FirstName = "clientFirstName"
            };

            var clientInfo = LoginResponseDtoMapper.MapClientLoginResponseToBasicUserInfo(loginResponse);

            Assert.IsNotNull (clientInfo);
            Assert.AreEqual(clientInfo.FirstName, loginResponse.FirstName);
            Assert.AreEqual(clientInfo.Id, loginResponse.Id);
            Assert.AreEqual("Client", clientInfo.Role);
        }

        [Test]
        public void TestMapClientLoginResponseToBasicUserInfoReturnsNull()
        {
            var clientInfo = LoginResponseDtoMapper.MapClientLoginResponseToBasicUserInfo(null);

            Assert.IsNull(clientInfo);
        }
    }
}
