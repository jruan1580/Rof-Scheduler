using AuthenticationService.Domain.Services;
using AuthenticationService.Infrastructure.EmployeeManagement;
using AuthenticationService.Infrastructure.EmployeeManagement.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Domain
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IEmployeeManagementAccessor> _employeeManagementAccessor;
        private Mock<ITokenHandler> _tokenHanlder;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();
            _tokenHanlder = new Mock<ITokenHandler>();

            _tokenHanlder.Setup(t => t.GenerateTokenForRole(It.IsAny<string>(), It.IsAny<int>())).Returns("tokentobeusedfortesting");
   
            _authService = new AuthService(_employeeManagementAccessor.Object, _tokenHanlder.Object);
        }

        [Test]
        public async Task TestEmployeeLoginSuccess()
        {
            _employeeManagementAccessor.Setup(e => e.CheckIfEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _employeeManagementAccessor.Setup(e => e.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new EmployeeLoginResponse()
                {
                    FirstName = "testFirstName",
                    Id = 1,
                    Role = "Administrator"
                });

            var basicUserInfo = await _authService.Login("testFirstName", "Password");

            Assert.AreEqual("testFirstName", basicUserInfo.FirstName);
            Assert.AreEqual("Administrator", basicUserInfo.Role);
            Assert.AreEqual(1, basicUserInfo.Id);
        }

        [Test]
        public async Task TestEmployeeLogoutSuccess()
        {
            _employeeManagementAccessor.Setup(e => e.Logout(It.IsAny<long>(), It.Is<string>(s => s.Equals("/api/Employee/logout/1")), It.IsAny<string>()))
                .ReturnsAsync(true);

            await _authService.Logout(1, "Employee", "testToken");

            _employeeManagementAccessor.Verify(e => e.Logout(It.IsAny<long>(), It.Is<string>(s => s.Equals("/api/Employee/logout/1")), It.IsAny<string>()), Times.Once);
        }
    }
}
