using ClientManagementService.API.Controllers;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientManagementService.Test.Controller
{
    [TestFixture]
    public class ClientControllerTest
    {
        private Mock<IClientService> _clientService;

        [SetUp]
        public void Setup()
        {
            _clientService = new Mock<IClientService>();
        }

        [Test]
        public async Task CreateClient_Success()
        {
            var newClient = new API.DTO.ClientDTO()
            {
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "jdoe@gmail.com",
                Password = "TestPassword123!",
                PrimaryPhoneNum = "123-456-7890",
                IsLoggedIn = false,
                IsLocked = false,
                FailedLoginAttempts = 0,
                TempPasswordChanged = false,
                Address = new API.DTO.AddressDTO()
                {
                    AddressLine1 = "123 Test St",
                    City = "San Diego",
                    State = "CA",
                    ZipCode = "12345"
                }
            };

            _clientService.Setup(c => c.CreateClient(It.IsAny<Domain.Models.Client>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.CreateClient(newClient);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(StatusCodeResult), response.GetType());

            var statusCode = (StatusCodeResult)response;

            Assert.AreEqual(statusCode.StatusCode, 201);
        }

        [Test]
        public async Task CreateClient_InternalServerError()
        {
            var newClient = new API.DTO.ClientDTO()
            {
                CountryId = 0,
                FirstName = "",
                LastName = "",
                EmailAddress = "",
                Password = "",
                PrimaryPhoneNum = "",
                Address = null
            };

            _clientService.Setup(c => c.CreateClient(It.IsAny<Domain.Models.Client>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new ClientController(_clientService.Object);

            var response = await controller.CreateClient(newClient);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        //[Test]
        //public async Task GetAllEmployees_Success()
        //{
        //    var clients = new List<Domain.Models.Client>()
        //    {
        //        new Domain.Models.Client()
        //        {
        //            CountryId = 1,
        //            FirstName = "John",
        //            LastName = "Doe",
        //            EmailAddress = "jdoe@gmail.com",
        //            Password = new byte[100],
        //            PrimaryPhoneNum = "123-456-7890",
        //            IsLoggedIn = false,
        //            IsLocked = false,
        //            FailedLoginAttempts = 0,
        //            TempPasswordChanged = false,
        //            Address = new Domain.Models.Address()
        //            {
        //                AddressLine1 = "123 Test St",
        //                City = "San Diego",
        //                State = "CA",
        //                ZipCode = "12345"
        //            }
        //        }
        //    };

        //    _clientService.Setup(c => c.GetAllClientsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
        //        .ReturnsAsync(new Domain.Models.ClientsWithTotalPage(clients, 1));

        //    var controller = new ClientController(_clientService.Object);

        //    var response = await controller.GetAllClients(1, 10, "");

        //    Assert.NotNull(response);
        //    Assert.AreEqual(typeof(OkObjectResult), response.GetType());

        //    var okObj = (OkObjectResult)response;

        //    Assert.AreEqual(okObj.StatusCode, 200);
        //}

        //[Test]
        //public async Task GetAllEmployees_InternalServerError()
        //{
        //    _clientService.Setup(c => c.GetAllClientsByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
        //        .ThrowsAsync(new Exception());

        //    var controller = new ClientController(_clientService.Object);

        //    var response = await controller.GetAllClients(1, 10, "");

        //    Assert.NotNull(response);
        //    Assert.AreEqual(typeof(ObjectResult), response.GetType());

        //    var obj = (ObjectResult)response;

        //    Assert.AreEqual(obj.StatusCode, 500);
        //}

        [Test]
        public async Task GetClientById_Success()
        {
            _clientService.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync(new Domain.Models.Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = new byte[100],
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var controller = new ClientController(_clientService.Object);

            var response = await controller.GetClientById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetClientById_InternalServerError()
        {
            _clientService.Setup(c => c.GetClientById(It.IsAny<long>()))
                .ReturnsAsync((Domain.Models.Client)null);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.GetClientById(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetClientByEmail_Success()
        {
            _clientService.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = new byte[100],
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var controller = new ClientController(_clientService.Object);

            var response = await controller.GetClientByEmail("jdoe@gmail.com");

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetClientByEmail_NotFoundError()
        {
            _clientService.Setup(c => c.GetClientByEmail(It.IsAny<string>()))
                .ReturnsAsync((Domain.Models.Client)null);

            var controller = new ClientController(_clientService.Object);

            var response = await controller.GetClientByEmail("jdoe@gmail.com");

            Assert.NotNull(response);
            Assert.AreEqual(typeof(NotFoundObjectResult), response.GetType());

            var obj = (NotFoundObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 404);
        }

        [Test]
        public async Task ClientLogin_Success()
        {
            _clientService.Setup(e => e.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.Client()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "jdoe@gmail.com",
                    Password = new byte[100],
                    PrimaryPhoneNum = "123-456-7890",
                    IsLoggedIn = false,
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false
                });

            var controller = new ClientController(_clientService.Object);

            var response = await controller.ClientLogin(new API.DTO.ClientDTO()
            {
                Username = "jdoe",
                Password = "TestPassword123!"
            });

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkObjectResult), response.GetType());

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task ClientLogin_InternalServerError()
        {
            _clientService.Setup(c => c.ClientLogin(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new ClientController(_clientService.Object);

            var response = await controller.ClientLogin(new API.DTO.ClientDTO()
            {
                Username = "jdoe",
                Password = "TestPassword123!"
            });

            Assert.NotNull(response);
            Assert.AreEqual(typeof(ObjectResult), response.GetType());

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
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
