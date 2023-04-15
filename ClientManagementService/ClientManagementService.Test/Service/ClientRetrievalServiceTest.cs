using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Entities;
using ClientManagementService.Infrastructure.Persistence.Filters.Client;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class ClientRetrievalServiceTest
    {
        [Test]
        public async Task GetAllClients_NoClients()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            clientRetrievalRepo.Setup(c =>
                c.GetAllClientsByKeyword(
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<string>()))
            .ReturnsAsync((new List<Client>(), 0));

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            var result = await clientService.GetAllClientsByKeyword(1, 10, "");

            Assert.IsEmpty(result.Clients);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetAllClients_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            var clients = new List<Client>()
            {
                ClientCreator.GetDbClient(Encoding.UTF8.GetBytes("password"))
            };

            clientRetrievalRepo.Setup(c =>
                c.GetAllClientsByKeyword(
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<string>()))
            .ReturnsAsync((clients, 0));

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            var result = await clientService.GetAllClientsByKeyword(1, 10, "");

            Assert.IsNotEmpty(result.Clients);
            AssertClientExpectedEqualsActualValues(result.Clients[0]);
        }

        [Test]
        public void GetClientById_NotFound()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            clientRetrievalRepo.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync((Client)null);

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => clientService.GetClientById(2));
        }

        [Test]
        public async Task GetClientById_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            clientRetrievalRepo.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<long>>()))
                .ReturnsAsync(ClientCreator.GetDbClient(Encoding.UTF8.GetBytes("password")));

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            var client = await clientService.GetClientById(1);

            AssertClientExpectedEqualsActualValues(client);
        }

        [Test]
        public void GetClientByEmail_NotFound()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            clientRetrievalRepo.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync((Client)null);

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => clientService.GetClientByEmail("abcd@gmail.com"));
        }

        [Test]
        public async Task GetClientByEmail_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            clientRetrievalRepo.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync(ClientCreator.GetDbClient(Encoding.UTF8.GetBytes("password")));

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            var client = await clientService.GetClientByEmail("test@email.com");

            AssertClientExpectedEqualsActualValues(client);
        }

        [Test]
        public void GetClientByUsername_NotFount()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            clientRetrievalRepo.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync((Client)null);

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => clientService.GetClientByUsername("jdoe"));
        }

        [Test]
        public async Task GetClientByUsername_Success()
        {
            var clientRetrievalRepo = new Mock<IClientRetrievalRepository>();

            clientRetrievalRepo.Setup(c => c.GetClientByFilter(It.IsAny<GetClientFilterModel<string>>()))
                .ReturnsAsync(ClientCreator.GetDbClient(Encoding.UTF8.GetBytes("password")));

            var clientService = new ClientRetrievalService(clientRetrievalRepo.Object);

            var client = await clientService.GetClientByUsername("jdoe");

            AssertClientExpectedEqualsActualValues(client);
        }

        private void AssertClientExpectedEqualsActualValues(Domain.Models.Client client)
        {
            Assert.IsNotNull(client);
            Assert.AreEqual("John", client.FirstName);
            Assert.AreEqual("Doe", client.LastName);
            Assert.AreEqual("jdoe", client.Username);
            Assert.AreEqual("test@email.com", client.EmailAddress);
        }
    }
}
