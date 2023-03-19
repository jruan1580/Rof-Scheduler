using EmployeeManagementService.Domain.Exceptions;
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
    public class EmployeeControllerTest : ApiTestSetup
    {
        private readonly string _passwordUnencrypted = "t3$T1234";

        private readonly string _baseUrl = "/api/Employee";

        private readonly string _exceptionMsg = "Test Exception Message";

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            SetAuthHeaderOnHttpClient("Employee");

            var stringContent = new StringContent(JsonConvert.SerializeObject(updateEmployee), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/info", stringContent);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
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
                .ThrowsAsync(new ArgumentException(_exceptionMsg));

            SetAuthHeaderOnHttpClient("Employee");

            var stringContent = new StringContent(JsonConvert.SerializeObject(updateEmployee), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/info", stringContent);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(errMsg, _exceptionMsg);
        }

        [Test]
        public async Task GetEmployeesForDropdown_Success()
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeesForDropdown())
            .ReturnsAsync(new List<Employee>()
            {
                new Employee()
                {
                    Id = 1,
                    FullName = "Test User"
                }
            });

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.GetAsync($"{_baseUrl}/employees");

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task GetEmployeeById_Success()
        {
            _employeeRetrievalService.Setup(e => 
                e.GetEmployeeById(It.IsAny<long>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.GetAsync($"{_baseUrl}/1");

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task GetEmployeeById_InternalServerError()
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync((Employee)null);

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.GetAsync($"{_baseUrl}/1");

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Test]
        public async Task GetEmployeeByUsername_Success()
        {
            _employeeRetrievalService.Setup(e => 
                e.GetEmployeeByUsername(It.IsAny<string>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.GetAsync($"{_baseUrl}/jdoe/username");

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task GetEmployeeByUsername_InternalServerError()
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.GetAsync($"{_baseUrl}/jdoe/username");

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee not found!", errMsg);
        }

        [Test]
        public async Task EmployeeLogin_Success()
        {
            _employeeAuthService.Setup(e => 
                e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            SetAuthHeaderOnHttpClient("Employee");

            var employeeDTO = new EmployeeDTO()
            {
                Username = "jdoe",
                Password = "test"
            };

            var response = await _httpClient.PatchAsync($"{_baseUrl}/login", new StringContent(JsonConvert.SerializeObject(employeeDTO), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task EmployeeLogin_InternalServerError()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            SetAuthHeaderOnHttpClient("Employee");

            var employeeDTO = new EmployeeDTO()
            {
                Username = "jdoe",
                Password = "test"
            };

            var response = await _httpClient.PatchAsync($"{_baseUrl}/login", new StringContent(JsonConvert.SerializeObject(employeeDTO), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(errMsg, _exceptionMsg);
        }

        [Test]
        public async Task EmployeeLogin_Locked()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
               .ThrowsAsync(new EmployeeIsLockedException());

            SetAuthHeaderOnHttpClient("Employee");

            var employeeDTO = new EmployeeDTO()
            {
                Username = "jdoe",
                Password = "test"
            };

            var response = await _httpClient.PatchAsync($"{_baseUrl}/login", new StringContent(JsonConvert.SerializeObject(employeeDTO), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var errorMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee is locked. Contact Admin to get unlocked.", errorMsg);
        }

        [Test]
        public async Task EmployeeLogout_Success()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.PatchAsync($"{_baseUrl}/1/logout", null);

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task EmployeeLogout_InternalServerError()
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.PatchAsync($"{_baseUrl}/1/logout", null);

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(errMsg, _exceptionMsg);
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

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.PatchAsync($"{_baseUrl}/password", new StringContent(JsonConvert.SerializeObject(password), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
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
                .ThrowsAsync(new Exception(_exceptionMsg));

            SetAuthHeaderOnHttpClient("Employee");

            var response = await _httpClient.PatchAsync($"{_baseUrl}/password", new StringContent(JsonConvert.SerializeObject(password), Encoding.UTF8, "application/json"));

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(errMsg, _exceptionMsg);
        }
    }
}
