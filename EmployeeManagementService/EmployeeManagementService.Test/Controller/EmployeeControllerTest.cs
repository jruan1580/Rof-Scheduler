using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.DTO;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
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
    }
}
