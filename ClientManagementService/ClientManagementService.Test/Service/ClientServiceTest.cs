using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class ClientServiceTest
    {
        private Mock<IClientRepository> _clientRepository;
        private IPasswordService _passwordService;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _clientRepository = new Mock<IClientRepository>();

            _config = new Mock<IConfiguration>();
            _config.Setup(c => c.GetSection(It.Is<string>(p => p.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public void CreateClient_InvalidInput()
        {
            var address = new Domain.Models.Address()
            {
                AddressLine1 = "",
                City = "",
                State = "",
                ZipCode = ""
            };

            var newClient = new Domain.Models.Client()
            {
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                PrimaryPhoneNum = "",
                Password = null,
                Address = address
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "TestPassword123!"));
        }

        [Test]
        public void CreateClient_EmailNameNotUnique()
        {
            var address = new Domain.Models.Address()
            {
                AddressLine1 = "123 Test St",
                City = "San Diego",
                State = "CA",
                ZipCode = "12345"
            };

            var newClient = new Domain.Models.Client()
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = address
            };

            _clientRepository.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync(new Client() { FirstName = "John", LastName = "Doe", EmailAddress = "jdoe@gmail.com"});

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "TestPassword123!"));
        }

        [Test]
        public void CreateClient_PasswordReqNotMet()
        {
            var address = new Domain.Models.Address()
            {
                AddressLine1 = "123 Test St",
                City = "San Diego",
                State = "CA",
                ZipCode = "12345"
            };

            var newClient = new Domain.Models.Client()
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                PrimaryPhoneNum = "123-456-7890",
                Password = null,
                Address = address
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "abc123"));
        }

        [Test]
        public async Task CreateClient_Success()
        {
            var address = new Domain.Models.Address()
            {
                AddressLine1 = "123 Test St",
                City = "San Diego",
                State = "CA",
                ZipCode = "12345"
            };

            var newClient = new Domain.Models.Client()
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = address
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.CreateClient(newClient, "TestPassword123!");

            _clientRepository.Verify(c => c.CreateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void UpdateClientInfo_InvaldClient()
        {
            var address = new Domain.Models.Address()
            {
                AddressLine1 = "",
                City = "",
                State = "",
                ZipCode = ""
            };

            var client = new Domain.Models.Client()
            {
                Id = 0,
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                PrimaryPhoneNum = "",
                Password = null,
                Address = address
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdateClientInfo(client));
        }

        [Test]
        public void UpdateClientInfo_EmailNameNotUnique()
        {
            var address = new Domain.Models.Address()
            {
                AddressLine1 = "123 Test St",
                City = "San Diego",
                State = "CA",
                ZipCode = "12345"
            };

            var client = new Domain.Models.Client()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = address
            };

            _clientRepository.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync(new Client() { FirstName = "John", LastName = "Doe", EmailAddress = "jdoe@gmail.com" });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdateClientInfo(client));
        }

        [Test]
        public async Task UpdateClientInfo_Success()
        {
            var address = new Domain.Models.Address()
            {
                AddressLine1 = "123 Test St",
                City = "San Diego",
                State = "CA",
                ZipCode = "12345"
            };

            var client = new Domain.Models.Client()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = address
            };

            _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync(new Client() { Id = 1 });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.UpdateClientInfo(client);

            _clientRepository.Verify(c => c.UpdateClientInfo(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void GetClientById_DoesNotExist()
        {
            _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync((Client)null);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.GetClientById(1));
        }

        [Test]
        public async Task GetClientById_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    PrimaryPhoneNum = "123-456-7890",
                    Password = encryptedPass,
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345",
                    IsLocked = false,
                    IsLoggedIn = false,
                    TempPasswordChanged = false,
                    FailedLoginAttempts = 0
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            var client = await clientService.GetClientById(1);

            Assert.IsNotNull(client);
            Assert.AreEqual(1, client.Id);
            Assert.AreEqual(1, client.CountryId);
            Assert.AreEqual("John", client.FirstName);
            Assert.AreEqual("Doe", client.LastName);
            Assert.AreEqual("jdoe@gmail.com", client.EmailAddress);
            Assert.AreEqual("123-456-7890", client.PrimaryPhoneNum);
            Assert.AreEqual(encryptedPass, client.Password);
            Assert.IsFalse(client.IsLocked);
            Assert.AreEqual(0, client.FailedLoginAttempts);
            Assert.IsFalse(client.TempPasswordChanged);
            Assert.IsFalse(client.IsLoggedIn);
            Assert.AreEqual("John Doe", client.FullName);
            Assert.AreEqual("123 Test St", client.Address.AddressLine1);
            Assert.IsNull(client.Address.AddressLine2);
            Assert.AreEqual("San Diego", client.Address.City);
            Assert.AreEqual("CA", client.Address.State);
            Assert.AreEqual("12345", client.Address.ZipCode);
        }

        [Test]
        public void GetClientByEmail_DoesNotExist()
        {
            _clientRepository.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException());

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.GetClientByEmail("jdoe@gmail.com"));
        }

        [Test]
        public async Task GetClientByEmail_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    PrimaryPhoneNum = "123-456-7890",
                    Password = encryptedPass,
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345",
                    IsLocked = false,
                    IsLoggedIn = false,
                    TempPasswordChanged = false,
                    FailedLoginAttempts = 0
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            var client = await clientService.GetClientByEmail("jdoe@gmail.com");

            Assert.IsNotNull(client);
            Assert.AreEqual(1, client.Id);
            Assert.AreEqual(1, client.CountryId);
            Assert.AreEqual("John", client.FirstName);
            Assert.AreEqual("Doe", client.LastName);
            Assert.AreEqual("jdoe@gmail.com", client.EmailAddress);
            Assert.AreEqual("123-456-7890", client.PrimaryPhoneNum);
            Assert.AreEqual(encryptedPass, client.Password);
            Assert.IsFalse(client.IsLocked);
            Assert.AreEqual(0, client.FailedLoginAttempts);
            Assert.IsFalse(client.TempPasswordChanged);
            Assert.IsFalse(client.IsLoggedIn);
            Assert.AreEqual("John Doe", client.FullName);
            Assert.AreEqual("123 Test St", client.Address.AddressLine1);
            Assert.IsNull(client.Address.AddressLine2);
            Assert.AreEqual("San Diego", client.Address.City);
            Assert.AreEqual("CA", client.Address.State);
            Assert.AreEqual("12345", client.Address.ZipCode);
        }

        [Test]
        public void ClientLogIn_AlreadyLoggedIn()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = true,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.ClientLogin("jdoe@gmail.com", "TestPassword123!"));
        }

        [Test]
        public void ClientLogIn_PasswordDoesNotMatch()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.ClientLogin("jdoe@gmail.com", "Test123!"));
        }

        [Test]
        public async Task ClientLogIn_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.ClientLogin("jdoe@gmail.com", "TestPassword123!");

            _clientRepository.Verify(c => c.UpdateClientLoginStatus(It.IsAny<long>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public void ClientLogOut_AlreadyLoggedOut()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.ClientLogout(1));
        }

        [Test]
        public async Task ClientLogOut_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = encryptedPass,
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = true,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.ClientLogout(1);

            _clientRepository.Verify(c => c.UpdateClientLoginStatus(It.IsAny<long>(), It.IsAny<bool>()), Times.Once);
        }
    }
}
