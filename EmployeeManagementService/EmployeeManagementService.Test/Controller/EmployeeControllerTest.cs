using EmployeeManagementService.API.Controllers;
using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Controller
{
    [TestFixture]
    public class EmployeeControllerTest
    {
        private readonly Mock<IEmployeeAuthService> _employeeAuthService = new Mock<IEmployeeAuthService>();

        private readonly Mock<IEmployeeRetrievalService> _employeeRetrievalService = new Mock<IEmployeeRetrievalService>();

        private readonly Mock<IEmployeeUpsertService> _employeeUpsertService = new Mock<IEmployeeUpsertService>();

        private readonly string _passwordUnencrypted = "t3$T1234";


        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.UpdateEmployeeInformation(updateEmployee);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkResult));

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task UpdateEmployeeInformation_BadRequestError()
        {
            var updateEmployee = new EmployeeDTO()
            {
                Id = 0,
                FirstName = "",
                LastName = "",
                Ssn = "",
                Username = "",
                Role = "",
                Address = new AddressDTO 
                { 
                    AddressLine1 = "", 
                    AddressLine2 = "", 
                    City = "",
                    State = "",
                    ZipCode = "" 
                }
            };

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .ThrowsAsync(new ArgumentException("bad arguments"));

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.UpdateEmployeeInformation(updateEmployee);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(BadRequestObjectResult));

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }

        [Test]
        public async Task GetEmployeeById_Success()
        {
            _employeeRetrievalService.Setup(e => 
                e.GetEmployeeById(It.IsAny<long>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.GetEmployeeById(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetEmployeeById_InternalServerError()
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync((Employee)null);

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.GetEmployeeById(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetEmployeeByUsername_Success()
        {
            _employeeRetrievalService.Setup(e => 
                e.GetEmployeeByUsername(It.IsAny<string>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.GetEmployeeByUsername("jdoe");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetEmployeeByUsername_InternalServerError()
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.GetEmployeeByUsername("jdoe");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(NotFoundObjectResult));

            var obj = (NotFoundObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 404);
        }

        [Test]
        public async Task EmployeeLogin_Success()
        {
            _employeeAuthService.Setup(e => 
                e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.EmployeeLogin(new EmployeeDTO()
            {
                Username = "jdoe",
                Password = "teST1234!"
            });

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var ok = (OkObjectResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task EmployeeLogin_InternalServerError()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.EmployeeLogin(new EmployeeDTO()
            {
                Username = "jdoe",
                Password = "abcdef123345!"
            });

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task EmployeeLogin_Locked()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
               .ThrowsAsync(new EmployeeIsLockedException());

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.EmployeeLogin(new EmployeeDTO()
            {
                Username = "jdoe",
                Password = "abcdef123345!"
            });

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(BadRequestObjectResult));

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }

        [Test]
        public async Task EmployeeLogout_Success()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["User-Agent"] = "Chrome";

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.EmployeeLogout(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkResult));

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task EmployeeLogout_InternalServerError()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .ThrowsAsync(new Exception());

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.EmployeeLogout(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task UpdatePassword_Success()
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _employeeUpsertService.Setup(e => e.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.UpdatePassword(password);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkResult));

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task UpdatePassword_InternalServerError()
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _employeeUpsertService.Setup(e => e.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new EmployeeController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await controller.UpdatePassword(password);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }
    }
}
