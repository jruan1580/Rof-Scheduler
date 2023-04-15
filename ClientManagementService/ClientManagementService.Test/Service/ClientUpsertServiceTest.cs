using ClientManagementService.Domain.Models;
using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RofShared.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();
            var clientUpsertRepo = new Mock<IClientUpsertRepository>();

            var newClient = ClientCreator.GetDomainClient(_passwordService.EncryptPassword(_passwordUnencrypted));
            newClient.Password = null;

            clientRetrievalRepo.Setup(e =>
                e.DoesClientExistByEmailOrUsername(It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .ReturnsAsync(true);

            var clientService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => clientService.CreateEmployee(newClient, _passwordUnencrypted));
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
    }
}
