using EmployeeManagementService.API.Controllers;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Controller
{
    [TestFixture]
    public class DropdownControllerTest
    {
        private Mock<IDropdownService> _dropdownService;

        [SetUp]
        public void Setup()
        {
            _dropdownService = new Mock<IDropdownService>();

            _dropdownService.Setup(d => d.GetEmployees())
                .ReturnsAsync(new List<Employee>()
                {
                    new Employee()
                    {
                        Id = 1,
                        FullName = "Test User"
                    }
                });
        }

        [Test]
        public async Task GetEmployeeTest()
        {
            var controller = new DropdownController(_dropdownService.Object);

            var result = await controller.GetEmployees();

            Assert.NotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var okObj = (OkObjectResult)result;

            Assert.AreEqual(okObj.StatusCode, 200);
        }
    }
}
