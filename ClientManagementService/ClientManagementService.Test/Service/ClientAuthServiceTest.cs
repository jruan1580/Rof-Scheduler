using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using RofShared.Services;
using ClientManagementService.Domain.Exceptions;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class ClientAuthServiceTest
    {
        private IPasswordService _passwordService;

        private Mock<IConfiguration> _config;

        private readonly string _passwordUnencrypted = "TestPassword123!";


        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>();

            _config.Setup(c => c.GetSection(It.Is<string>(p => p.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public async Task ClientLogIn_AlreadyLoggedIn()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var dbClient = ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbClient.IsLoggedIn = true;

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
            .ReturnsAsync(dbClient);

            var clientService = new ClientAuthService(clientRetrievalRepo.Object, clientUpsertRepo.Object, _passwordService);

            var client = await clientService.ClientLogin("jdoe", "TestPassword123!");

            Assert.IsTrue(client.IsLoggedIn);
        }

        [Test]
        public void ClientLogIn_PasswordDoesNotMatch()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var dbClient = ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbClient.IsLoggedIn = false;

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
            .ReturnsAsync(dbClient);

            short failedAttempts = 3;
            clientUpsertRepo.Setup(c => c.IncrementClientFailedLoginAttempts(It.IsAny<long>()))
                .ReturnsAsync(failedAttempts);

            var clientService = new ClientAuthService(clientRetrievalRepo.Object, clientUpsertRepo.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.ClientLogin("jdoe", "Test123!"));
            clientUpsertRepo.Verify(c => c.IncrementClientFailedLoginAttempts(It.Is<long>(id => id == 1)), Times.Once);
            clientUpsertRepo.Verify(c => c.UpdateClient(It.Is<Client>(c => c.Id == 1 && c.FailedLoginAttempts == 3 && c.IsLocked == true)), Times.Once);
        }

        [Test]
        public void ClientLogIn_Locked()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var dbClient = ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbClient.IsLoggedIn = false;
            dbClient.IsLocked = true;

            clientRetrievalRepo.Setup(c =>
               c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
           .ReturnsAsync(dbClient);

            var clientService = new ClientAuthService(clientRetrievalRepo.Object, clientUpsertRepo.Object, _passwordService);

            Assert.ThrowsAsync<ClientIsLockedException>(() => clientService.ClientLogin("jdoe", _passwordUnencrypted));
        }

        [Test]
        public async Task ClientLogIn_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var dbClient = ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbClient.IsLoggedIn = false;

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
            .ReturnsAsync(dbClient);

            var clientService = new ClientAuthService(clientRetrievalRepo.Object, clientUpsertRepo.Object, _passwordService);

            await clientService.ClientLogin("jdoe", _passwordUnencrypted);

            clientUpsertRepo.Verify(c => c.UpdateClient(It.Is<Client>(c => c.IsLoggedIn == true)), Times.Once);
        }

        [Test]
        public async Task ClientLogOut_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var dbClient = ClientCreator.GetDbClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbClient.IsLoggedIn = true;

            clientRetrievalRepo.Setup(c =>
                c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
            .ReturnsAsync(dbClient);

            var clientService = new ClientAuthService(clientRetrievalRepo.Object, clientUpsertRepo.Object, _passwordService);

            await clientService.ClientLogout(1);

            clientUpsertRepo.Verify(c => c.UpdateClient(It.Is<Client>(c => !c.IsLoggedIn)), Times.Once);
        }
    }
}
