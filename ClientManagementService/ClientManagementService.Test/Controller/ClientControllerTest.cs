using ClientManagementService.Domain.Models;
using ClientManagementService.DTO;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Controller
{
    [TestFixture]
    public class ClientControllerTest : ApiTestSetup
    {
        private readonly string _passwordUnencrypted = "t3$T1234";

        private readonly string _baseUrl = "/api/Client";

        private readonly string _exceptionMsg = "Test Exception Message";

        [Test]
        public async Task CreateClient_Success()
        {
            var newClient = ClientCreator.GetClientDTO("Client");

            _clientUpsertService.Setup(c =>
                c.CreateClient(It.IsAny<Client>(),
                    It.IsAny<string>()))
            .Returns(Task.CompletedTask);

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newClient));

            AssertExpectedStatusCode(response, HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateClient_BadRequestError()
        {
            var newClient = new ClientDTO()
            {
                CountryId = 0,
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                Password = "",
                PrimaryPhoneNum = "",
                Address = null
            };

            _clientUpsertService.Setup(c => 
                c.CreateClient(It.IsAny<Client>(), 
                    It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newClient));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task GetAllClients_Success()
        {
            var clients = new List<Client>()
            {
                ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted))
            };

            _clientRetrievalService.Setup(c =>
                c.GetAllClientsByKeyword(It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()))
            .ReturnsAsync(new ClientsWithTotalPage(clients, 1));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}?page=1&offset=10&keyword=test");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetAllClients_InternalServerError()
        {
            _clientRetrievalService.Setup(c =>
                c.GetAllClientsByKeyword(It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()))
            .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}?page=1&offset=10&keyword=test");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task GetClientById_Success()
        {
            _clientRetrievalService.Setup(c =>
                c.GetClientById(It.IsAny<long>()))
            .ReturnsAsync(ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetClientById_NotFound()
        {
            _clientRetrievalService.Setup(c =>
                c.GetClientById(It.IsAny<long>()))
            .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task GetClientById_InternalServerError()
        {
            _clientRetrievalService.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync((Client)null);

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetClientByEmail_Success()
        {
            _clientRetrievalService.Setup(c =>
                c.GetClientByEmail(It.IsAny<string>()))
            .ReturnsAsync(ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/test@gmail.com/email");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetClientByEmail_NotFound()
        {
            _clientRetrievalService.Setup(c =>
                c.GetClientByEmail(It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/test@gmail.com/email");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task ClientLogin_Success()
        {
            var client = ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted));

            _clientAuthService.Setup(c =>
                c.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(client);

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/login", ConvertObjectToStringContent(client));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task ClientLogin_NotFound()
        {
            var client = ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted));

            _clientAuthService.Setup(c =>
                c.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/login", ConvertObjectToStringContent(client));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task ClientLogin_LockedOut()
        {
            var client = ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted));

            _clientAuthService.Setup(c =>
                c.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("Client account is locked. Contact admin to get unlocked."));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/login", ConvertObjectToStringContent(client));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, "Client account is locked. Contact admin to get unlocked.");
        }

        [Test]
        public async Task ClientLogin_InternalServerError()
        {
            var client = ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted));

            _clientAuthService.Setup(c => c.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/login", ConvertObjectToStringContent(client));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task ClientLogout_Success()
        {
            _clientAuthService.Setup(c => c.ClientLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/1/logout");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task ClientLogout_NotFound()
        {
            _clientAuthService.Setup(c => c.ClientLogout(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/1/logout");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task ClientLogout_InternalServerError()
        {
            _clientAuthService.Setup(c => c.ClientLogout(It.IsAny<long>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/1/logout");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task ResetLockedStatus_Success()
        {
            _clientUpsertService.Setup(c => c.ResetClientFailedLoginAttempts(1))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Administrator", HttpMethod.Patch, $"{_baseUrl}/1/locked");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task ResetLockedStatus_NotFound()
        {
            _clientUpsertService.Setup(c => c.ResetClientFailedLoginAttempts(1))
                .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Administrator", HttpMethod.Patch, $"{_baseUrl}/1/locked");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task ResetLockedStatus_InternalServerError()
        {
            _clientUpsertService.Setup(c => c.ResetClientFailedLoginAttempts(1))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Patch, $"{_baseUrl}/1/locked");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdateClientInfo_Success()
        {
            var updateClient = ClientCreator.GetClientDTO("Client");
            updateClient.Id = 1;

            _clientUpsertService.Setup(c => c.UpdateClientInformation(It.IsAny<Client>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateClient));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdateClientInfo_NotFound()
        {
            var updateClient = ClientCreator.GetClientDTO("Client");
            updateClient.Id = 1;

            _clientUpsertService.Setup(c => c.UpdateClientInformation(It.IsAny<Client>()))
                .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateClient));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task UpdateClientInfo_BadRequestError()
        {
            var updateClient = ClientCreator.GetClientDTO("Client");
            updateClient.Id = 1;

            _clientUpsertService.Setup(c => c.UpdateClientInformation(It.IsAny<Client>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateClient));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdateClientInfo_InternalServerError()
        {
            var updateClient = ClientCreator.GetClientDTO("Client");
            updateClient.Id = 1;

            _clientUpsertService.Setup(c => c.UpdateClientInformation(It.IsAny<Client>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateClient));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdatePassword_Success()
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _clientUpsertService.Setup(c => c.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/password", ConvertObjectToStringContent(password));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdatePassword_NotFound()
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _clientUpsertService.Setup(c => c.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{ _baseUrl}/password", ConvertObjectToStringContent(password));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task UpdatePassword_InternalServerError()
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _clientUpsertService.Setup(c => c.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/password", ConvertObjectToStringContent(password));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task DeleteClientById_Success()
        {
            _clientUpsertService.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task DeleteClientById_NotFound()
        {
            _clientUpsertService.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task DeleteClientById_BadRequestError()
        {
            _clientUpsertService.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task DeleteClientById_InternalServerError()
        {
            _clientUpsertService.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Delete, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }
    }
}
