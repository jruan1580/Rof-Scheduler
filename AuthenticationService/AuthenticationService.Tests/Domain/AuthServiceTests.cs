using AuthenticationService.Domain.Services;
using AuthenticationService.Infrastructure.ClientManagement;
using AuthenticationService.Infrastructure.EmployeeManagement;
using AuthenticationService.Infrastructure.Models;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Domain
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IEmployeeManagementAccessor> _employeeManagementAccessor;
        private Mock<IClientManagementAccessor> _clientManagementAccessor;
        private Mock<ITokenHandler> _tokenHanlder;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();
            _clientManagementAccessor = new Mock<IClientManagementAccessor>();
            _tokenHanlder = new Mock<ITokenHandler>();

            _tokenHanlder.Setup(t => t.GenerateTokenForRole(It.IsAny<string>(), It.IsAny<int>())).Returns("tokentobeusedfortesting");
   
            _authService = new AuthService(_employeeManagementAccessor.Object, _clientManagementAccessor.Object, _tokenHanlder.Object);
        }

        [Test]
        public async Task TestEmployeeLoginSuccess()
        {
            _employeeManagementAccessor.Setup(e => e.CheckIfEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _employeeManagementAccessor.Setup(e => e.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
        public void TestEmployeeLoginNotFound()
        {
            _employeeManagementAccessor.Setup(e => e.CheckIfEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _employeeManagementAccessor.Setup(e => e.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((EmployeeLoginResponse)null);

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Login("testFirstName", "password"));
        }

        [Test]
        public async Task TestClientLoginSuccess()
        {
            _employeeManagementAccessor.Setup(e => e.CheckIfEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            _clientManagementAccessor.Setup(e => e.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new LoginResponse()
                {
                    FirstName = "testFirstName",
                    Id = 1
                });

            var basicUserInfo = await _authService.Login("testFirstName", "Password");

            Assert.AreEqual("testFirstName", basicUserInfo.FirstName);
            Assert.AreEqual("Client", basicUserInfo.Role);
            Assert.AreEqual(1, basicUserInfo.Id);
        }

        [Test]
        public void TesClientLoginNotFound()
        {
            _employeeManagementAccessor.Setup(e => e.CheckIfEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            _clientManagementAccessor.Setup(e => e.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((LoginResponse)null);

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Login("testFirstName", "password"));
        }

        [Test]
        public async Task TestEmployeeLogoutSuccess()
        {
            _employeeManagementAccessor.Setup(e =>
                e.Logout(It.IsAny<long>(), It.Is<string>(s => s.Equals("/api/Employee/1/logout")), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

            await _authService.Logout(1, "Employee");

            _employeeManagementAccessor.Verify(e => e.Logout(It.IsAny<long>(), It.Is<string>(s => s.Equals("/api/Employee/1/logout")), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TestEmployeeLogoutNotFound()
        {
            _employeeManagementAccessor.Setup(e => 
                e.Logout(It.IsAny<long>(), It.Is<string>(s => s.Equals("/api/Employee/1/logout")), It.IsAny<string>()))
              .ThrowsAsync(new EntityNotFoundException("Employee"));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Logout(1, "Employee"));
        }

        [Test]
        public async Task TestClientLogoutSuccess()
        {
            _clientManagementAccessor.Setup(e => e.Logout(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _authService.Logout(1, "Client");

            _clientManagementAccessor.Verify(e => e.Logout(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TestClientLogoutNotFound()
        {
            _clientManagementAccessor.Setup(e => e.Logout(It.IsAny<long>(), It.IsAny<string>()))
               .ThrowsAsync(new EntityNotFoundException("Client"));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Logout(1, "Client"));
        }
    }
}
