using AuthenticationService.Domain.Services;
using AuthenticationService.DTO.Accessors;
using AuthenticationService.Infrastructure.EmployeeManagement;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Domain
{
    [TestFixture]
    public class EmployeeAuthHelperTests
    {
        private Mock<IEmployeeManagementAccessor> _employeeManagementAccessor;

        private readonly string _username = "testEmployee";
        private readonly string _password = "testPassword";
        private readonly string _token = "tokens";
        private readonly long _employeeId = 1;
        private readonly string _employeeFirstName = "Client First Name";
        private readonly string _employeeRole = "Employee";

        [Test]
        public async Task HandleEmployeeLoginSuccess()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();

            _employeeManagementAccessor.Setup(e =>
                e.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new EmployeeLoginResponse()
            {
                Id = _employeeId,
                FirstName = _employeeFirstName,
                Role = _employeeRole
            });

            var employeeAuthHelper = new EmployeeAuthHelper(_employeeManagementAccessor.Object);

            var employeeInfo = await employeeAuthHelper.HandleEmployeeLogin(_username, _password, _token);

            Assert.IsNotNull(employeeInfo);
            Assert.AreEqual(_employeeId, employeeInfo.Id);
            Assert.AreEqual(_employeeFirstName, employeeInfo.FirstName);
            Assert.AreEqual(_employeeRole, employeeInfo.Role);
        }

        [Test]
        public void HandleEmployeeLogin_EmployeeNotFound()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();

            _employeeManagementAccessor.Setup(e =>
                e.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException(_employeeRole));

            var employeeAuthHelper = new EmployeeAuthHelper(_employeeManagementAccessor.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => employeeAuthHelper.HandleEmployeeLogin(_username, _password, _token));
        }

        [Test]
        public async Task HandleEmployeeLogoutSuccess()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();

            _employeeManagementAccessor.Setup(e =>
                e.Logout(It.Is<long>(id => id == _employeeId), It.IsAny<string>(), It.Is<string>(token => token.Equals(_token))))
            .Returns(Task.CompletedTask);

            var employeeAuthHelper = new EmployeeAuthHelper(_employeeManagementAccessor.Object);

            await employeeAuthHelper.HandleEmployeeLogout(_employeeId, _employeeRole, _token);

            _employeeManagementAccessor.Verify(e =>
               e.Logout(It.Is<long>(id => id == _employeeId), It.IsAny<string>(), It.Is<string>(token => token.Equals(_token))), 
               Times.Once);
        }

        [Test]
        public void HandleEmployeeLogout_EmployeeNotFound()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();

            _employeeManagementAccessor.Setup(e =>
                e.Logout(It.Is<long>(id => id == _employeeId), It.IsAny<string>(), It.Is<string>(token => token.Equals(_token))))
            .ThrowsAsync(new EntityNotFoundException(_employeeRole));

            var employeeAuthHelper = new EmployeeAuthHelper(_employeeManagementAccessor.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => employeeAuthHelper.HandleEmployeeLogout(_employeeId, _employeeRole, _token));
        }

        [Test]
        public async Task CheckIfEmployeeReturnsTrue()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();

            _employeeManagementAccessor.Setup(e =>
                e.CheckIfEmployee(It.Is<string>(u => u.Equals(_username)), It.Is<string>(t => t.Equals(_token))))
            .ReturnsAsync(true);

            var employeeAuthHelper = new EmployeeAuthHelper(_employeeManagementAccessor.Object);
            var isAnEmployee = await employeeAuthHelper.IsAnEmployee(_username, _token);
            
            Assert.True(isAnEmployee);
        }

        [Test]
        public async Task CheckIfEmployeeReturnsFalse()
        {
            _employeeManagementAccessor = new Mock<IEmployeeManagementAccessor>();

            _employeeManagementAccessor.Setup(e =>
                e.CheckIfEmployee(It.Is<string>(u => u.Equals(_username)), It.Is<string>(t => t.Equals(_token))))
            .ReturnsAsync(false);

            var employeeAuthHelper = new EmployeeAuthHelper(_employeeManagementAccessor.Object);
            var isAnEmployee = await employeeAuthHelper.IsAnEmployee(_username, _token);

            Assert.False(isAnEmployee);
        }
    }
}
