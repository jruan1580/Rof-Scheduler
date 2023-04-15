using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RofShared.Exceptions;
using RofShared.Services;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class ClientServiceTest
    {
        private Mock<IClientRepository> _clientRepository;
        private Mock<IClientRetrievalRepository> _clientRetrievalRepository;
        private IPasswordService _passwordService;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _clientRepository = new Mock<IClientRepository>();
            _clientRetrievalRepository = new Mock<IClientRetrievalRepository>();

            _config = new Mock<IConfiguration>();
            _config.Setup(c => c.GetSection(It.Is<string>(p => p.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public void CreateClient_InvalidInput()
        {
            var newClient = new Domain.Models.Client()
            {
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                Username = "",
                PrimaryPhoneNum = "",
                Password = null,
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "",
                    City = "",
                    State = "",
                    ZipCode = ""
                }
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "TestPassword123!"));
        }

        [Test]
        public void CreateClient_EmailNameNotUnique()
        {
            var newClient = new Domain.Models.Client()
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientRetrievalRepository.Setup(c => c.DoesClientExistByEmailOrUsername(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "TestPassword123!"));
        }

        [Test]
        public void CreateClient_UsernameNotUnique()
        {
            var newClient = new Domain.Models.Client()
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientRetrievalRepository.Setup(c => c.DoesClientExistByEmailOrUsername(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "TestPassword123!"));
        }

        [Test]
        public void CreateClient_PasswordReqNotMet()
        {
            var newClient = new Domain.Models.Client()
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = null,
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateClient(newClient, "abc123"));
        }

        [Test]
        public async Task CreateClient_Success()
        {
            var newClient = new Domain.Models.Client()
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.CreateClient(newClient, "TestPassword123!");

            _clientRepository.Verify(c => c.CreateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void UpdateClientInfo_InvaldClient()
        {
            var client = new Domain.Models.Client()
            {
                Id = 0,
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                Username = "",
                PrimaryPhoneNum = "",
                Password = null,
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "",
                    City = "",
                    State = "",
                    ZipCode = ""
                }
            };

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdateClientInfo(client));
        }

        [Test]
        public void UpdateClientInfo_EmailNameNotUnique()
        {
            var client = new Domain.Models.Client()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientRetrievalRepository.Setup(c => c.DoesClientExistByEmailOrUsername(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdateClientInfo(client));
        }

        [Test]
        public void UpdateClientInfo_UsernameNotUnique()
        {
            var client = new Domain.Models.Client()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientRetrievalRepository.Setup(c => c.DoesClientExistByEmailOrUsername(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdateClientInfo(client));
        }

        [Test]
        public async Task UpdateClientInfo_Success()
        {
            var client = new Domain.Models.Client()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Username = "jdoe",
                PrimaryPhoneNum = "123-456-7890",
                Password = new byte[100],
                Address = new Domain.Models.Address()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientRetrievalRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync(new Client() { Id = 1 });

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.UpdateClientInfo(client);

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            var client = await clientService.ClientLogin("jdoe", "TestPassword123!");

            Assert.IsTrue(client.IsLoggedIn);
        }

        [Test]
        public void ClientLogIn_PasswordDoesNotMatch()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            var client = new Client()
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
            };

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync(client);

            short failedAttempts = 3;
            _clientRepository.Setup(c => c.IncrementClientFailedLoginAttempts(It.IsAny<long>()))
                .ReturnsAsync(failedAttempts);

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.ClientLogin("jdoe", "Test123!"));
            _clientRepository.Verify(c => c.IncrementClientFailedLoginAttempts(It.Is<long>(id => id == 1)), Times.Once);
            _clientRepository.Verify(c => c.UpdateClient(It.Is<Client>(c => c.Id == 1 && c.FailedLoginAttempts == 3 && c.IsLocked == true)), Times.Once);
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

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

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.ClientLogout(1);

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void ResetClientFailedLoginAttempt_ClientDoesNotExist()
        {
            _clientRetrievalRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync((Client)null);

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => clientService.ResetClientFailedLoginAttempts(0));
        }

        [Test]
        public async Task ResetClientFailedLoginAttempt_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

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
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false
                });

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.ResetClientFailedLoginAttempts(1);

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void UpdatePassword_RequirementsNotMet()
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdatePassword(1, "abcdefg"));
        }

        [Test]
        public void UpdatePassword_PasswordNotNew()
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdatePassword(1, "TestPassword123!"));
        }

        [Test]
        public async Task UpdatePassword_Success()
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.UpdatePassword(1, "TestPassword1234!");

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void DeleteClientById_NoClient()
        {
            _clientRepository.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.DeleteClientById(1));
        }

        [Test]
        public async Task DeleteClientById_Success()
        {
            _clientRepository.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var clientService = new ClientService(_clientRepository.Object, _passwordService, _clientRetrievalRepository.Object);

            await clientService.DeleteClientById(1);

            _clientRepository.Verify(c => c.DeleteClientById(It.IsAny<long>()), Times.Once);
        }
    }
}
