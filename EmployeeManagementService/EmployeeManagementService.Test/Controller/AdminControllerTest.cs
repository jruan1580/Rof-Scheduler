﻿using EmployeeManagementService.API.Controllers;
using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Controller
{
    [TestFixture]
    public class AdminControllerTest
    {
        private Mock<IEmployeeService> _employeeService;

        [SetUp]
        public void Setup()
        {
            _employeeService = new Mock<IEmployeeService>();
        }

        [Test]
        public async Task GetAllEmployees_Success()
        {
            var employees = new List<Domain.Models.Employee>()
            {
                new Domain.Models.Employee()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = new byte[32],
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                }
            };

            _employeeService.Setup(e => e.GetAllEmployeesByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.EmployeesWithTotalPage(employees, 1));

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.GetAllEmployees(1, 10, "");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(OkObjectResult));

            var okObj = (OkObjectResult)response;

            Assert.AreEqual(okObj.StatusCode, 200);
        }

        [Test]
        public async Task GetAllEmployees_InternalServerError()
        {
            _employeeService.Setup(e => e.GetAllEmployeesByKeyword(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.GetAllEmployees(1, 10, "");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task CreateEmployee_Success()
        {
            var newEmployee = new EmployeeDTO()
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "jdoe",
                Ssn = "123-45-6789",
                Password = "T3$T1234",
                Role = "Employee",
                Active = true
            };

            _employeeService.Setup(e => e.CreateEmployee(It.IsAny<Domain.Models.Employee>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.CreateEmployee(newEmployee);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(StatusCodeResult));

            var statusCode = (StatusCodeResult)response;

            Assert.AreEqual(statusCode.StatusCode, 201);
        }

        [Test]
        public async Task CreateEmployee_BadRequestError()
        {
            var newEmployee = new EmployeeDTO()
            {
                FirstName = "",
                LastName = "",
                Username = "",
                Ssn = "",
                Password = "",
                Role = "",
                Active = null
            };

            _employeeService.Setup(e => e.CreateEmployee(It.IsAny<Employee>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("bad arguments"));

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.CreateEmployee(newEmployee);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(BadRequestObjectResult), response.GetType());

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
        }

        [Test]
        public async Task ResetLockedStatus_Success()
        {
            _employeeService.Setup(e => e.ResetEmployeeFailedLoginAttempt(1))
                .Returns(Task.CompletedTask);

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.ResetLockedStatus(1);

            Assert.NotNull(response);
            Assert.AreEqual(typeof(OkResult), response.GetType());

            var ok = (OkResult)response;

            Assert.AreEqual(ok.StatusCode, 200);
        }

        [Test]
        public async Task ResetLockedStatus_InternalServerError()
        {
            _employeeService.Setup(e => e.ResetEmployeeFailedLoginAttempt(1))
                .ThrowsAsync(new Exception());

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.ResetLockedStatus(1);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var updateEmployee = new EmployeeDTO()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Ssn = "123-45-6789",
                Username = "jdoe",
                Role = "Employee",
                Address = new AddressDTO { AddressLine1 = "123 Abc St", AddressLine2 = "", City = "Oakland", State = "CA", ZipCode = "12345" }
            };

            _employeeService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Domain.Models.Employee>()))
                .Returns(Task.CompletedTask);

            var controller = new AdminController(_employeeService.Object);

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
                Address = new AddressDTO { AddressLine1 = "", AddressLine2 = "", City = "", State = "", ZipCode = "" }
            };

            _employeeService.Setup(_e => _e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .ThrowsAsync(new ArgumentException("bad arguments"));

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.UpdateEmployeeInformation(updateEmployee);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(BadRequestObjectResult));

            var obj = (BadRequestObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 400);
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

            var controller = new AdminController(_employeeService.Object);

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

            var controller = new AdminController(_employeeService.Object);

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

            var controller = new AdminController(_employeeService.Object);

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

            var controller = new AdminController(_employeeService.Object);

            var response = await controller.GetEmployeeByUsername("jdoe");

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(NotFoundObjectResult));

            var obj = (NotFoundObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 404);
        }

        [Test]
        public async Task EmployeeLogin_Success()
        {
            _employeeService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Domain.Models.Employee()
                {
                    Role = "Administrator"
                });
     

            var controller = new AdminController(_employeeService.Object);

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
            _employeeService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new AdminController(_employeeService.Object);

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
        public async Task AdminLogin_Locked()
        {
            _employeeService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
               .ThrowsAsync(new EmployeeIsLockedException());

            var controller = new AdminController(_employeeService.Object);

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
            _employeeService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var controller = new AdminController(_employeeService.Object);

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

            var controller = new AdminController(_employeeService.Object);

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

            _employeeService.Setup(e => e.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new EmployeeController(_employeeService.Object);

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

            _employeeService.Setup(e => e.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var controller = new EmployeeController(_employeeService.Object);

            var response = await controller.UpdatePassword(password);

            Assert.NotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ObjectResult));

            var obj = (ObjectResult)response;

            Assert.AreEqual(obj.StatusCode, 500);
        }
    }
}
