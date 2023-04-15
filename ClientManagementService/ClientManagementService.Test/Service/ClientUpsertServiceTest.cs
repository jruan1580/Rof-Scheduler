using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using RofShared.Services;
using System;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class ClientUpsertServiceTest
    {
        private IPasswordService _passwordService;

        private Mock<IConfiguration> _config;

        private readonly string _passwordUnencrypted = "TestPassword123!";

        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>();

            _config.Setup(c =>
                c.GetSection(It.Is<string>(r => r.Equals("PasswordSalt"))).Value)
            .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public void CreateClient_InvalidInput()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var newClient = new Domain.Models.Client()
            {
                FirstName = "",
                LastName = "",
                Username = "",
                EmailAddress = "",
                Password = null,
                PrimaryPhoneNum = ""
            };

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, _passwordUnencrypted));
        }

        [Test]
        public void CreateClient_EmailOrUsernameNotUnique()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var newClient = ClientCreator.GetDomainClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            newClient.Password = null;

            clientRetrievalRepo.Setup(e =>
                e.DoesClientExistByEmailOrUsername(It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .ReturnsAsync(true);

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, _passwordUnencrypted));
        }

        [Test]
        public void CreateClient_PasswordReqNotMet()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var newClient = ClientCreator.GetDomainClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            newClient.Password = null;

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "abc123"));
        }

        [Test]
        public async Task CreateClient_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var newClient = ClientCreator.GetDomainClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            newClient.Password = null;

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            await clientService.CreateClient(newClient, "TestPassword123!");

            clientUpsertRepo.Verify(c => 
                c.CreateClient(It.Is<Client>(c => c.FirstName == newClient.FirstName &&
                    c.LastName == newClient.LastName &&
                    c.EmailAddress == newClient.EmailAddress &&
                    c.Username == newClient.Username)), 
            Times.Once);
        }

        [Test]
        public void UpdateClientInfo_InvaldClient()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var updateClient = new Domain.Models.Client()
            {
                FirstName = "",
                LastName = "",
                Username = "",
                EmailAddress = "",
                Password = null,
                PrimaryPhoneNum = ""
            };

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdateClientInformation(updateClient));
        }

        [Test]
        public void UpdateClientInfo_EmailOrUsernameNotUnique()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var updateClient = ClientCreator.GetDomainClient(_passwordService.EncryptPassword(_passwordUnencrypted));

            clientRetrievalRepo.Setup(e =>
                e.DoesClientExistByEmailOrUsername(It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .ReturnsAsync(true);

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdateClientInformation(updateClient));
        }

        [Test]
        public async Task UpdateClientInformation_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var updateClient = ClientCreator.GetDomainClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            updateClient.Id = 1;

            clientRetrievalRepo.Setup(c => 
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync(ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted)));

            clientRetrievalRepo.Setup(e =>
                e.DoesClientExistByEmailOrUsername(It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .ReturnsAsync(false);

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            await clientService.UpdateClientInformation(updateClient);

            clientUpsertRepo.Verify(c => c.UpdateClient(It.Is<Client>(
                c => c.FirstName == updateClient.FirstName &&
                    c.LastName == updateClient.LastName &&
                    c.EmailAddress == updateClient.EmailAddress &&
                    c.Username == updateClient.Username)), 
            Times.Once);
        }

        [Test]
        public void ResetClientFailedLoginAttempt_NotFound()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync((Client)null);

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<EntityNotFoundException>(() => clientService.ResetClientFailedLoginAttempts(0));
        }

        [Test]
        public async Task ResetClientFailedLoginAttempt_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var client = ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            client.IsLocked = true;
            client.FailedLoginAttempts = 3;

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync(client);

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            await clientService.ResetClientFailedLoginAttempts(1);

            clientUpsertRepo.Verify(c =>
                c.UpdateClient(It.Is<Client>(c =>
                    !c.IsLocked &&
                    c.FailedLoginAttempts == 0)),
            Times.Once);
        }

        [Test]
        public void UpdatePassword_RequirementsNotMet()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync(ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdatePassword(1, "abcdefg"));
        }

        [Test]
        public void UpdatePassword_PasswordNotNew()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync(ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdatePassword(1, _passwordUnencrypted));
        }

        [Test]
        public async Task UpdatePassword_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync(ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            await clientService.UpdatePassword(1, "tE$t1234");

            clientUpsertRepo.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void DeleteClientById_NoClient()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync((Client)null);

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            clientUpsertRepo.Verify(c => c.DeleteClientById(It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task DeleteClientById_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            clientUpsertRepo.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync(ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var clientService = new ClientUpsertService(clientRetrievalRepo.Object,
                clientUpsertRepo.Object,
                _passwordService);

            await clientService.DeleteClientById(1);

            clientUpsertRepo.Verify(c => c.DeleteClientById(It.Is<long>(id => id == 1)), Times.Once);
        }
    }
}
