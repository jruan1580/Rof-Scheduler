using AuthenticationService.Domain.Model;
using AuthenticationService.Domain.Services;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Domain
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IEmployeeAuthHelper> _employeeAuthHelper;
        private Mock<IClientAuthHelper> _clientAuthHelper;
        private Mock<ITokenHandler> _tokenHanlder;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _employeeAuthHelper = new Mock<IEmployeeAuthHelper>();
            _clientAuthHelper = new Mock<IClientAuthHelper>();
            _tokenHanlder = new Mock<ITokenHandler>();

            _tokenHanlder.Setup(t => t.GenerateTokenForRole(It.IsAny<string>(), It.IsAny<int>())).Returns("tokentobeusedfortesting");
   
            _authService = new AuthService(_tokenHanlder.Object, _employeeAuthHelper.Object, _clientAuthHelper.Object);
        }

        [Test]
        public async Task TestEmployeeLoginSuccess()
        {
            _employeeAuthHelper.Setup(e => e.IsAnEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _employeeAuthHelper.Setup(e => e.HandleEmployeeLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new BasicUserInfo()
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
            _employeeAuthHelper.Setup(e => e.IsAnEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _employeeAuthHelper.Setup(e => e.HandleEmployeeLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Login("testFirstName", "password"));
        }

        [Test]
        public async Task TestClientLoginSuccess()
        {
            _employeeAuthHelper.Setup(e => e.IsAnEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            _clientAuthHelper.Setup(e => e.HandleClientLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new BasicUserInfo()
                {
                    FirstName = "testFirstName",
                    Role = "Client",
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
            _employeeAuthHelper.Setup(e => e.IsAnEmployee(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            _clientAuthHelper.Setup(e => e.HandleClientLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Client"));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Login("testFirstName", "password"));
        }

        [Test]
        public async Task TestEmployeeLogoutSuccess()
        {
            _employeeAuthHelper.Setup(e =>
                e.HandleEmployeeLogout(It.IsAny<long>(), It.Is<string>(s => s.Equals("Employee")), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

            await _authService.Logout(1, "Employee");

            _employeeAuthHelper.Verify(e => e.HandleEmployeeLogout(It.IsAny<long>(), It.Is<string>(s => s.Equals("Employee")), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TestEmployeeLogoutNotFound()
        {
            _employeeAuthHelper.Setup(e => 
                e.HandleEmployeeLogout(It.IsAny<long>(), It.Is<string>(s => s.Equals("Employee")), It.IsAny<string>()))
              .ThrowsAsync(new EntityNotFoundException("Employee"));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Logout(1, "Employee"));
        }

        [Test]
        public async Task TestClientLogoutSuccess()
        {
            _clientAuthHelper.Setup(e => e.HandleClientLogout(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _authService.Logout(1, "Client");

            _clientAuthHelper.Verify(e => e.HandleClientLogout(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TestClientLogoutNotFound()
        {
            _clientAuthHelper.Setup(e => e.HandleClientLogout(It.IsAny<long>(), It.IsAny<string>()))
               .ThrowsAsync(new EntityNotFoundException("Client"));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.Logout(1, "Client"));
        }
    }
}
