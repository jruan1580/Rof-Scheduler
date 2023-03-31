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
    public class AEmployeeControllerTests : ApiTestSetup
    {
        private readonly string _passwordUnencrypted = "t3$T1234";

        private readonly string _exceptionMsg = "Test Exception Message";

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeById_Success(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e =>
                e.GetEmployeeById(It.IsAny<long>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.GetAsync($"{baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeById_NotFound(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.GetAsync($"{baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee not found!", errMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeById_InternalServerError(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync((Employee)null);

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.GetAsync($"{baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeByUsername_Success(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e =>
                e.GetEmployeeByUsername(It.IsAny<string>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.GetAsync($"{baseUrl}/jdoe/username");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeByUsername_NotFound(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.GetAsync($"{baseUrl}/jdoe/username");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee not found!", errMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeByUsername_InternalServerErrors(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ReturnsAsync((Employee)null);

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.GetAsync($"{baseUrl}/jdoe/username");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeesForDropdown_Success(string baseUrl, string role)
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

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.GetAsync($"{baseUrl}/employees");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogin_Success(string baseUrl, string role)
        {
            var employee = EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted));
            employee.Role =role;

            _employeeAuthService.Setup(e =>
                e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(employee);

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/login", new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json"));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogin_NotFound(string baseUrl, string role)
        {
            var employee = EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted));
            employee.Role = role;

            _employeeAuthService.Setup(e =>
                e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException("Employee"));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/login", new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json"));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee not found!", errMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogin_IsLockedOut(string baseUrl, string role)
        {
            var employee = EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted));
            employee.Role = role;

            _employeeAuthService.Setup(e =>
                e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new EmployeeIsLockedException());

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/login", new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json"));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            Assert.IsNotNull(response.Content);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee is locked. Contact Admin to get unlocked.", errMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogin_InternalServerError(string baseUrl, string role)
        {
            var employee = EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted));
            employee.Role = role;

            _employeeAuthService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/login", new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json"));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            var errorMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(_exceptionMsg, errorMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogout_Success(string baseUrl, string role)
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/1/logout", null);

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogout_NotFound(string baseUrl, string role)
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/1/logout", null);

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee not found!", errMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogout_InternalServerError(string baseUrl, string role)
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/1/logout", null);

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(errMsg, _exceptionMsg);
        }


        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task UpdatePassword_Success(string baseUrl, string role)
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _employeeUpsertService.Setup(e => e.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/password", new StringContent(JsonConvert.SerializeObject(password), Encoding.UTF8, "application/json"));

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task UpdatePassword_NotFound(string baseUrl, string role)
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _employeeUpsertService.Setup(e => e.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/password", new StringContent(JsonConvert.SerializeObject(password), Encoding.UTF8, "application/json"));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee not found!", errMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task UpdatePassword_InternalServerError(string baseUrl, string role)
        {
            var password = new PasswordDTO()
            {
                Id = 1,
                NewPassword = "NewTestPassword123!"
            };

            _employeeUpsertService.Setup(e => e.UpdatePassword(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            SetAuthHeaderOnHttpClient(role);

            var response = await _httpClient.PatchAsync($"{baseUrl}/password", new StringContent(JsonConvert.SerializeObject(password), Encoding.UTF8, "application/json"));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(_exceptionMsg, content);
        }
    }
}
