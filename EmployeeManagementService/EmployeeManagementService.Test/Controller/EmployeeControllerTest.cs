using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.DTO;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Controller
{
    [TestFixture]
    public class EmployeeControllerTest : ApiTestSetup
    {
        private readonly string _baseUrl = "/api/Employee";

        private readonly string _exceptionMsg = "Test Exception Message";

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            var response = await SendRequest("Employee", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));           

            AssertExpectedStatusCode(response, HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdateEmployeeInformation_NotFound()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .ThrowsAsync(new EntityNotFoundException("Employee"));

            var response = await SendRequest("Employee", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.NotFound);

            Assert.IsNotNull(response.Content);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Employee not found!", errMsg);
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

            var response = await SendRequest("Employee", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.BadRequest);
          
            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(errMsg, _exceptionMsg);
        }

        [Test]
        public async Task UpdateEmployeeInformation_InternalServerError()
        {
            var updateEmployee = EmployeeCreator.GetEmployeeDTO("Employee");
            updateEmployee.Id = 1;

            _employeeUpsertService.Setup(e => e.UpdateEmployeeInformation(It.IsAny<Employee>()))
                .ThrowsAsync(new Exception(_exceptionMsg));

            var response = await SendRequest("Employee", HttpMethod.Put, $"{_baseUrl}/info", ConvertObjectToStringContent(updateEmployee));

            AssertExpectedStatusCode(response, HttpStatusCode.InternalServerError);

            var errMsg = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(errMsg, _exceptionMsg);
        }
    }
}
