using ClientManagementService.Domain.Models;
using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RofShared.Services;
using System;
using System.Threading.Tasks;
using DBClient = ClientManagementService.Infrastructure.Persistence.Entities.Client;

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

            var newClient = new Client()
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
                c.CreateClient(It.Is<DBClient>(c => c.FirstName == newClient.FirstName &&
                    c.LastName == newClient.LastName &&
                    c.EmailAddress == newClient.EmailAddress &&
                    c.Username == newClient.Username)), 
            Times.Once);
        }
    }
}
