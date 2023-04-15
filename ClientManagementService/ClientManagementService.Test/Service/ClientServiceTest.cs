using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using RofShared.Exceptions;
using RofShared.Services;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class ClientServiceTest
    {
        private Mock<IClientUpsertRepository> _clientRepository;
        private Mock<IClientRetrievalRepository> _clientRetrievalRepository;
        private IPasswordService _passwordService;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _clientRepository = new Mock<IClientUpsertRepository>();
            _clientRetrievalRepository = new Mock<IClientRetrievalRepository>();

            _config = new Mock<IConfiguration>();
            _config.Setup(c => c.GetSection(It.Is<string>(p => p.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public async Task ClientLogIn_AlreadyLoggedIn()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRetrievalRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Username = "jdoe",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = true,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientAuthService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            var client = await clientService.ClientLogin("jdoe", "TestPassword123!");

            Assert.IsTrue(client.IsLoggedIn);
        }

        [Test]
        public void ClientLogIn_PasswordDoesNotMatch()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRetrievalRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Username = "jdoe",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            short failedAttempts = 2;
            _clientRepository.Setup(c => c.IncrementClientFailedLoginAttempts(It.IsAny<long>()))
                .ReturnsAsync(failedAttempts);

            var clientService = new ClientAuthService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.ClientLogin("jdoe", "Test123!"));
        }

        [Test]
        public async Task ClientLogIn_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRetrievalRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Username = "jdoe",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientAuthService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.ClientLogin("jdoe", "TestPassword123!");

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task ClientLogOut_AlreadyLoggedOut()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRetrievalRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Username = "jdoe",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientRetrievalService(_clientRetrievalRepository.Object);

            var client = await clientService.GetClientById(1);

            Assert.IsFalse(client.IsLoggedIn);
        }

        [Test]
        public async Task ClientLogOut_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRetrievalRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Username = "jdoe",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = true,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientAuthService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.ClientLogout(1);

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }
    }
}
