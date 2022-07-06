using ClientManagementService.Domain.Exceptions;
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            _clientRepository.Setup(c => c.ClientAlreadyExists(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            _clientRepository.Setup(c => c.ClientAlreadyExists(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            _clientRepository.Setup(c => c.ClientAlreadyExists(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            _clientRepository.Setup(c => c.ClientAlreadyExists(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(true);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

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

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync(new Client() { Id = 1 });

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.UpdateClientInfo(client);

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void GetClientById_DoesNotExist()
        {
            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync((Client)null);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ClientNotFoundException>(() => clientService.GetClientById(1));
        }

        [Test]
        public async Task GetClientById_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    PrimaryPhoneNum = "123-456-7890",
                    Username = "jdoe",
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
            Assert.AreEqual("jdoe", client.Username);
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
        public async Task GetAllClients_NoClients()
        {
            _clientRepository.Setup(c => c.GetAllClientsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((new List<Client>(), 0));

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            var result = await clientService.GetAllClientsByKeyword(1, 10, "");

            Assert.IsEmpty(result.Clients);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetAllClients_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");
            var clients = new List<Client>()
            {
                new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    PrimaryPhoneNum = "123-456-7890",
                    Username = "jdoe",
                    Password = encryptedPass,
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345",
                    IsLocked = false,
                    IsLoggedIn = false,
                    TempPasswordChanged = false,
                    FailedLoginAttempts = 0
                }
            };

            _clientRepository.Setup(c => c.GetAllClientsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((clients, 1));

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            var result = await clientService.GetAllClientsByKeyword(1, 10, "");

            Assert.IsNotEmpty(result.Clients);
            Assert.AreEqual("John", result.Clients[0].FirstName);
            Assert.AreEqual("Doe", result.Clients[0].LastName);
            Assert.AreEqual("123-456-7890", result.Clients[0].PrimaryPhoneNum);
            Assert.AreEqual("jdoe@gmail.com", result.Clients[0].EmailAddress);
            Assert.AreEqual("jdoe", result.Clients[0].Username);
            Assert.AreEqual(encryptedPass, result.Clients[0].Password);
            Assert.AreEqual("123 Test St", result.Clients[0].Address.AddressLine1);
            Assert.AreEqual("San Diego", result.Clients[0].Address.City);
            Assert.AreEqual("CA", result.Clients[0].Address.State);
            Assert.AreEqual("12345", result.Clients[0].Address.ZipCode);
            Assert.IsFalse(result.Clients[0].IsLocked);
            Assert.IsFalse(result.Clients[0].TempPasswordChanged);
            Assert.IsFalse(result.Clients[0].IsLoggedIn);
        }

        [Test]
        public void GetClientByEmail_DoesNotExist()
        {
            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ThrowsAsync(new ArgumentException());

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.GetClientByEmail("jdoe@gmail.com"));
        }

        [Test]
        public async Task GetClientByEmail_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync(new Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Username = "jdoe",
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
            Assert.AreEqual("jdoe", client.Username);
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
        public async Task ClientLogIn_AlreadyLoggedIn()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            var client = await clientService.ClientLogin("jdoe", "TestPassword123!");

            Assert.IsTrue(client.IsLoggedIn);
        }

        [Test]
        public void ClientLogIn_PasswordDoesNotMatch()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
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

            short failedAttempts = 1;
            _clientRepository.Setup(c => c.IncrementClientFailedLoginAttempts(It.IsAny<long>()))
                .ReturnsAsync(failedAttempts);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.ClientLogin("jdoe", "Test123!"));
        }

        [Test]
        public async Task ClientLogIn_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.ClientLogin("jdoe", "TestPassword123!");

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task ClientLogOut_AlreadyLoggedOut()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            var client = await clientService.GetClientById(1);

            Assert.IsFalse(client.IsLoggedIn);
        }

        [Test]
        public async Task ClientLogOut_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.ClientLogout(1);

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        //[Test]
        //public void IncrementClientFailedLoginAttempt_ClientDoesNotExist()
        //{
        //    _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
        //        .ReturnsAsync((Client)null);

        //    var clientService = new ClientService(_clientRepository.Object, _passwordService);

        //    Assert.ThrowsAsync<ArgumentException>(() => clientService.IncrementClientFailedLoginAttempts(0));
        //}

        //[Test]
        //public async Task IncrementClientFailedLoginAttempt_AccountAlreadyLocked()
        //{
        //    var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

        //    _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
        //        .ReturnsAsync(new Client()
        //        {
        //            Id = 1,
        //            CountryId = 1,
        //            FirstName = "John",
        //            LastName = "Doe",
        //            EmailAddress = "jdoe@gmail.com",
        //            Password = encryptedPass,
        //            PrimaryPhoneNum = "123-456-7890",
        //            IsLoggedIn = false,
        //            IsLocked = true,
        //            FailedLoginAttempts = 3,
        //            TempPasswordChanged = false
        //        });

        //    var clientService = new ClientService(_clientRepository.Object, _passwordService);

        //    var client = await clientService.GetClientById(1);
        //    await clientService.IncrementClientFailedLoginAttempts(client.Id);

        //    Assert.IsTrue(client.IsLocked);
        //}

        //[Test]
        //public async Task IncrementClientFailedLoginAttempt_AttemptsNot3()
        //{
        //    var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

        //    _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
        //        .ReturnsAsync(new Client()
        //        {
        //            Id = 1,
        //            CountryId = 1,
        //            FirstName = "John",
        //            LastName = "Doe",
        //            EmailAddress = "jdoe@gmail.com",
        //            Password = encryptedPass,
        //            PrimaryPhoneNum = "123-456-7890",
        //            IsLoggedIn = false,
        //            IsLocked = false,
        //            FailedLoginAttempts = 2,
        //            TempPasswordChanged = false
        //        });

        //    var clientService = new ClientService(_clientRepository.Object, _passwordService);

        //    var client = await clientService.GetClientById(1);

        //    _clientRepository.Setup(c => c.IncrementClientFailedLoginAttempts(It.Is<long>(i => i.Equals(client.Id))))
        //        .ReturnsAsync(client.FailedLoginAttempts + 1);

        //    await clientService.IncrementClientFailedLoginAttempts(client.Id);

        //    Assert.AreNotEqual(3, client.FailedLoginAttempts);
        //}

        //[Test]
        //public async Task IncrementClientFailedLoginAttempt_Success()
        //{
        //    var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

        //    _clientRepository.Setup(c => c.GetClientById(It.IsAny<long>()))
        //        .ReturnsAsync(new Client()
        //        {
        //            Id = 1,
        //            CountryId = 1,
        //            FirstName = "John",
        //            LastName = "Doe",
        //            EmailAddress = "jdoe@gmail.com",
        //            Password = encryptedPass,
        //            PrimaryPhoneNum = "123-456-7890",
        //            IsLoggedIn = false,
        //            IsLocked = false,
        //            FailedLoginAttempts = 1,
        //            TempPasswordChanged = false
        //        });

        //    var clientService = new ClientService(_clientRepository.Object, _passwordService);

        //    var client = await clientService.GetClientById(1);

        //    _clientRepository.Setup(c => c.IncrementClientFailedLoginAttempts(It.Is<long>(i => i.Equals(client.Id))))
        //        .ReturnsAsync(client.FailedLoginAttempts + 1);

        //    await clientService.IncrementClientFailedLoginAttempts(client.Id);

        //    Assert.AreEqual(1, client.FailedLoginAttempts);

        //    _clientRepository.Verify(c => c.IncrementClientFailedLoginAttempts(It.IsAny<long>()), Times.Once);
        //}

        [Test]
        public void ResetClientFailedLoginAttempt_ClientDoesNotExist()
        {
            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync((Client)null);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ClientNotFoundException>(() => clientService.ResetClientFailedLoginAttempts(0));
        }

        [Test]
        public async Task ResetClientFailedLoginAttempt_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.ResetClientFailedLoginAttempts(1);

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void UpdatePassword_RequirementsNotMet()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdatePassword(1, "abcdefg"));
        }

        [Test]
        public void UpdatePassword_PasswordNotNew()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.UpdatePassword(1, "TestPassword123!"));
        }

        [Test]
        public async Task UpdatePassword_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            _clientRepository.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
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

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.UpdatePassword(1, "TestPassword1234!");

            _clientRepository.Verify(c => c.UpdateClient(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void DeleteClientById_NoClient()
        {
            _clientRepository.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.DeleteClientById(1));
        }

        [Test]
        public async Task DeleteClientById_Success()
        {
            _clientRepository.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var clientService = new ClientService(_clientRepository.Object, _passwordService);

            await clientService.DeleteClientById(1);

            _clientRepository.Verify(c => c.DeleteClientById(It.IsAny<long>()), Times.Once);
        }
    }
}
