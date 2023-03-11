using AuthenticationService.Domain.Services;
using AuthenticationService.DTO.Accessors;
using AuthenticationService.Infrastructure.ClientManagement;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Domain
{
    [TestFixture]
    public class ClientAuthHelperTests
    {
        private Mock<IClientManagementAccessor> _clientManagementAccessor;

        private readonly string _username = "testClient";
        private readonly string _password = "testPassword";
        private readonly string _token = "tokens";
        private readonly long _clientId = 1;
        private readonly string _clientFirstName = "Client First Name";
        private readonly string _clientRole = "Client";

        [Test]
        public async Task HandleClientLoginSuccess()
        {
            _clientManagementAccessor = new Mock<IClientManagementAccessor>();

            _clientManagementAccessor.Setup(c =>
                c.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new LoginResponse()
            {
                Id = _clientId,
                FirstName = _clientFirstName
            });

            var clientAuthHelper = new ClientAuthHelper(_clientManagementAccessor.Object);

            var clientInfo = await clientAuthHelper.HandleClientLogin(_username, _password, _token);

            Assert.IsNotNull(clientInfo);
            Assert.AreEqual(_clientId, clientInfo.Id);
            Assert.AreEqual(_clientFirstName, clientInfo.FirstName);
            Assert.AreEqual(_clientRole, clientInfo.Role);
        }

        [Test]
        public void HandleClientLogin_ClientNotFound()
        {
            _clientManagementAccessor = new Mock<IClientManagementAccessor>();

            _clientManagementAccessor.Setup(c =>
                c.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException(_clientRole));

            var clientAuthHelper = new ClientAuthHelper(_clientManagementAccessor.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => clientAuthHelper.HandleClientLogin(_username, _password, _token));
        }

        [Test]
        public async Task HandleClientLogoutSuccess()
        {
            _clientManagementAccessor = new Mock<IClientManagementAccessor>();

            _clientManagementAccessor.Setup(c =>
                c.Logout(It.Is<long>(id => id == _clientId), It.Is<string>(token => token.Equals(_token))))
            .Returns(Task.CompletedTask);

            var clientAuthHelper = new ClientAuthHelper(_clientManagementAccessor.Object);

            await clientAuthHelper.HandleClientLogout(_clientId, _token);

            _clientManagementAccessor.Verify(c =>
                c.Logout(It.Is<long>(id => id == _clientId), It.Is<string>(token => token.Equals(_token))), Times.Once);
        }
        
        [Test]
        public void HandleClientLogout_ClientNotFound()
        {
            _clientManagementAccessor = new Mock<IClientManagementAccessor>();

            _clientManagementAccessor.Setup(c =>
                c.Logout(It.Is<long>(id => id == _clientId), It.Is<string>(token => token.Equals(_token))))
            .ThrowsAsync(new EntityNotFoundException(_clientRole));

            var clientAuthHelper = new ClientAuthHelper(_clientManagementAccessor.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => clientAuthHelper.HandleClientLogout(_clientId, _token));
        }
    }
}
