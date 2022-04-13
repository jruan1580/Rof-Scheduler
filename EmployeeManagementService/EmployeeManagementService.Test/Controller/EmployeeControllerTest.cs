using EmployeeManagementService.API.Controllers;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Controller
{
    [TestFixture]
    public class EmployeeControllerTest
    {
        private Mock<IEmployeeService> _employeeService;

        [SetUp]
        public void Setup()
        {
            _employeeService = new Mock<IEmployeeService>();
        }

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var updateEmployee = new API.DTO.EmployeeDTO()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Ssn = "123-45-6789",
                Username = "jdoe",
                Role = "Employee",
                Address = new API.DTO.AddressDTO { AddressLine1 = "123 Abc St", AddressLine2 = "", City = "Oakland", State = "CA", ZipCode = "12345" }
            };

            _employeeService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Domain.Models.Employee>()))
                .Returns(Task.CompletedTask);

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.UpdateEmployeeInformation(updateEmployee);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkResult));

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task UpdateEmployeeInformation_InternalServerError()
        {
            var updateEmployee = new API.DTO.EmployeeDTO()
            {
                Id = 0,
                FirstName = "",
                LastName = "",
                Ssn = "",
                Username = "",
                Role = "",
                Address = new API.DTO.AddressDTO { AddressLine1 = "", AddressLine2 = "", City = "", State = "", ZipCode = "" }
            };

            _employeeService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Domain.Models.Employee>()))
                .ThrowsAsync(new Exception());

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.UpdateEmployeeInformation(updateEmployee);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetEmployeeById_Success()
        {
            _employeeService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(new Domain.Models.Employee()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = new byte[100],
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                });

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.GetEmployeeById(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetEmployeeById_InternalServerError()
        {
            _employeeService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync((Domain.Models.Employee)null);

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.GetEmployeeById(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task GetEmployeeByUsername_Success()
        {
            _employeeService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.Employee()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = new byte[100],
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                });

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.GetEmployeeByUsername("jdoe");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetEmployeeByUsername_InternalServerError()
        {
            _employeeService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ReturnsAsync((Domain.Models.Employee)null);

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.GetEmployeeByUsername("jdoe");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task EmployeeLogin_Success()
        {
            _employeeService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.EmployeeLogin(new API.DTO.EmployeeDTO()
            {
                Username = "jdoe",
                Password = "teST1234!"
            });

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkResult));

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task EmployeeLogin_InternalServerError()
        {
            _employeeService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.EmployeeLogin(new API.DTO.EmployeeDTO()
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
        public async Task EmployeeLogout_Success()
        {
            _employeeService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.EmployeeLogout(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkResult));

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task EmployeeLogout_InternalServerError()
        {
            _employeeService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .ThrowsAsync(new Exception());

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.EmployeeLogout(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }
    }
}
