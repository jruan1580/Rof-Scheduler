using ClientManagementService.API.Controllers;
using ClientManagementService.API.DTO;
using ClientManagementService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
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

            _clientService.Setup(c =>
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

            _clientService.Setup(c => 
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

            _clientService.Setup(c =>
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
            _clientService.Setup(c =>
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
            _clientService.Setup(c =>
                c.GetClientById(It.IsAny<long>()))
            .ReturnsAsync(ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetClientById_NotFound()
        {
            _clientService.Setup(c =>
                c.GetClientById(It.IsAny<long>()))
            .ThrowsAsync(new EntityNotFoundException("Client"));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _clientNotFoundMessage);
        }

        [Test]
        public async Task GetClientById_InternalServerError()
        {
            _clientService.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync((Client)null);

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetClientByEmail_Success()
        {
            _clientService.Setup(c =>
                c.GetClientByEmail(It.IsAny<string>()))
            .ReturnsAsync(ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var response = await SendRequest("Client", HttpMethod.Get, $"{_baseUrl}/test@gmail.com/email");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetClientByEmail_NotFound()
        {
            _clientService.Setup(c =>
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

            _clientService.Setup(c =>
                c.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(client);

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/login", ConvertObjectToStringContent(client));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task ClientLogin_NotFound()
        {
            var client = ClientCreator.GetDomainClient(Encoding.UTF8.GetBytes(_passwordUnencrypted));

            _clientService.Setup(c =>
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

            _clientService.Setup(c =>
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

            _clientService.Setup(c => c.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Client", HttpMethod.Patch, $"{_baseUrl}/login", ConvertObjectToStringContent(client));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task ClientLogout_Success()
        {
            _clientService.Setup(c => c.ClientLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.ClientLogout(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task ClientLogout_InternalServerError()
        {
            _clientService.Setup(c => c.ClientLogout(It.IsAny<long>()))
                .ThrowsAsync(new Exception());

            var controller = new ClientController(_clientService.Object);

            var response = await controller.ClientLogout(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task ResetClientLockedStatus_Success()
        {
            _clientService.Setup(c => c.ResetClientFailedLoginAttempts(1))
                .Returns(Task.CompletedTask);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.ResetClientLockedStatus(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task ResetClientLockedStatus_BadRequestError()
        {
            _clientService.Setup(c => c.ResetClientFailedLoginAttempts(1))
                .ThrowsAsync(new ArgumentException());

            var controller = new ClientController(_clientService.Object);

            var response = await controller.ResetClientLockedStatus(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }

        [Test]
        public async Task UpdateClientInfo_Success()
        {
            var client = new API.DTO.ClientDTO()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                PrimaryPhoneNum = "123-456-7890",
                Address = new API.DTO.AddressDTO()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientService.Setup(c => c.UpdateClientInfo(It.IsAny<Domain.Models.Client>()))
                .Returns(Task.CompletedTask);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.UpdateClientInfo(client);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task UpdateClientInfo_BadRequestError()
        {
            var client = new API.DTO.ClientDTO()
            {
                Id = 0,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                PrimaryPhoneNum = "123-456-7890",
                Address = new API.DTO.AddressDTO()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientService.Setup(c => c.UpdateClientInfo(It.IsAny<Domain.Models.Client>()))
                .ThrowsAsync(new ArgumentException());

            var controller = new ClientController(_clientService.Object);

            var response = await controller.UpdateClientInfo(client);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }

        [Test]
        public async Task UpdatePassword_Success()
        {
            var password = new API.DTO.PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _clientService.Setup(c => c.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.UpdatePassword(password);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task UpdatePassword_InternalServerError()
        {
            var password = new API.DTO.PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _clientService.Setup(c => c.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new ClientController(_clientService.Object);

            var response = await controller.UpdatePassword(password);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task DeleteClientById_Success()
        {
            _clientService.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.DeleteClientById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task DeleteClientById_BadRequestError()
        {
            _clientService.Setup(c => c.DeleteClientById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var controller = new ClientController(_clientService.Object);

            var response = await controller.DeleteClientById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }
    }
}
