using EmployeeManagementService.API.Controllers;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.DTO;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Controller
{
    [TestFixture]
    public class AdminControllerTest : ApiTestSetup
    {
        private readonly string _passwordUnencrypted = "t3$T1234";

        private readonly string _baseUrl = "/api/Admin";

        private readonly string _exceptionMsg = "Test Exception Message";
                   

        [Test]
        public async Task GetAllEmployees_Success()
        {
            var employees = new List<Employee>()
            {
                EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted))
            };

            _employeeRetrievalService.Setup(e => 
                e.GetAllEmployeesByKeyword(It.IsAny<int>(), 
                    It.IsAny<int>(), 
                    It.IsAny<string>()))
            .ReturnsAsync(new EmployeesWithTotalPage(employees, 1));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}?page=1&offset=10&keyword=test");           

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task GetAllEmployees_InternalServerError()
        {           
            _employeeRetrievalService.Setup(e =>
                e.GetAllEmployeesByKeyword(It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()))
            .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Get, $"{_baseUrl}?page=1&offset=10&keyword=test");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task CreateEmployee_Success()
        {
            var newEmployee = EmployeeCreator.GetEmployeeDTO("Employee");

            _employeeUpsertService.Setup(e => 
                e.CreateEmployee(It.IsAny<Employee>(), 
                    It.IsAny<string>()))
            .Returns(Task.CompletedTask);                      

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newEmployee));            

            AssertExpectedStatusCode(response, HttpStatusCode.Created);
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

            _employeeUpsertService.Setup(e => 
                e.CreateEmployee(It.IsAny<Employee>(), 
                    It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var response = await SendRequest("Administrator", HttpMethod.Post, _baseUrl, ConvertObjectToStringContent(newEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task ResetLockedStatus_Success()
        {
            _employeeUpsertService.Setup(e => e.ResetEmployeeFailedLoginAttempt(1))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Administrator", HttpMethod.Patch, $"{_baseUrl}/1/locked");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task ResetLockedStatus_NotFound()
        {
            _employeeUpsertService.Setup(e => e.ResetEmployeeFailedLoginAttempt(1))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            var response = await SendRequest("Administrator", HttpMethod.Patch, $"{_baseUrl}/1/locked");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            AssertContentIsAsExpected(response, _employeeNotFoundMessage);
        }

        [Test]
        public async Task ResetLockedStatus_InternalServerError()
        {
            _employeeUpsertService.Setup(e => e.ResetEmployeeFailedLoginAttempt(1))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var controller = new AdminController(_employeeAuthService.Object, _employeeRetrievalService.Object, _employeeUpsertService.Object);

            var response = await SendRequest("Administrator", HttpMethod.Patch, $"{_baseUrl}/1/locked");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);          

            var stringContent = new StringContent(JsonConvert.SerializeObject(updateEmployee), Encoding.UTF8, "application/json");

            var response = await SendRequest("Administrator", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdateEmployeeInformation_NotFound()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            var stringContent = new StringContent(JsonConvert.SerializeObject(updateEmployee), Encoding.UTF8, "application/json");

            var response = await SendRequest("Administrator", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _employeeNotFoundMessage);
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

            _employeeUpsertService.Setup(_e => _e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            var stringContent = new StringContent(JsonConvert.SerializeObject(updateEmployee), Encoding.UTF8, "application/json");

            var response = await SendRequest("Administrator", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [Test]
        public async Task UpdateEmployeeInformation_InternalServerError()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var stringContent = new StringContent(JsonConvert.SerializeObject(updateEmployee), Encoding.UTF8, "application/json");

            var response = await SendRequest("Administrator", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }
    }
}
