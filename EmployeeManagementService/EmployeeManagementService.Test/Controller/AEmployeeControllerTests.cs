using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.DTO;
using Moq;
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

            var response = await SendRequest(role, HttpMethod.Get, $"{baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeById_NotFound(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            var response = await SendRequest(role, HttpMethod.Get, $"{baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _employeeNotFoundMessage);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeById_InternalServerError(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync((Employee)null);

            var response = await SendRequest(role, HttpMethod.Get, $"{baseUrl}/1");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeByUsername_Success(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e =>
                e.GetEmployeeByUsername(It.IsAny<string>()))
            .ReturnsAsync(EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted)));

            var response = await SendRequest(role, HttpMethod.Get, $"{baseUrl}/jdoe/username");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeByUsername_NotFound(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            var response = await SendRequest(role, HttpMethod.Get, $"{baseUrl}/jdoe/username");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _employeeNotFoundMessage);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task GetEmployeeByUsername_InternalServerErrors(string baseUrl, string role)
        {
            _employeeRetrievalService.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ReturnsAsync((Employee)null);

            var response = await SendRequest(role, HttpMethod.Get, $"{baseUrl}/jdoe/username");

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

            var response = await SendRequest(role, HttpMethod.Get, $"{baseUrl}/employees");

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

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/login", ConvertObjectToStringContent(employee));

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

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/login", ConvertObjectToStringContent(employee));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _employeeNotFoundMessage);
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

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/login", ConvertObjectToStringContent(employee));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);

            AssertContentIsAsExpected(response, "Employee is locked. Contact Admin to get unlocked.");
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogin_InternalServerError(string baseUrl, string role)
        {
            var employee = EmployeeCreator.GetDomainEmployee(Encoding.UTF8.GetBytes(_passwordUnencrypted));
            employee.Role = role;

            _employeeAuthService.Setup(e => e.EmployeeLogIn(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/login", ConvertObjectToStringContent(employee));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogout_Success(string baseUrl, string role)
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/1/logout");

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogout_NotFound(string baseUrl, string role)
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/1/logout");

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _employeeNotFoundMessage);
        }

        [TestCase("/api/Admin", "Administrator")]
        [TestCase("/api/Employee", "Employee")]
        public async Task EmployeeLogout_InternalServerError(string baseUrl, string role)
        {
            _employeeAuthService.Setup(e => e.EmployeeLogout(It.IsAny<long>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/1/logout");

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
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

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/password", ConvertObjectToStringContent(password));

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

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/password", ConvertObjectToStringContent(password));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            AssertContentIsAsExpected(response, _employeeNotFoundMessage);
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

            var response = await SendRequest(role, HttpMethod.Patch, $"{baseUrl}/password", ConvertObjectToStringContent(password));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            AssertContentIsAsExpected(response, _exceptionMsg);
        }
    }
}
