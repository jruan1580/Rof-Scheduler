using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Service
{
    [TestFixture]
    public class DropdownServiceTest
    {
        private Mock<IEmployeeRepository> _employeeRepository; 

        private DropdownService _dropdownService;

        [SetUp]
        public void Setup()
        {
            _employeeRepository = new Mock<IEmployeeRepository>();

            _employeeRepository.Setup(e => e.GetEmployeesForDropdown())
                .ReturnsAsync(new List<Employee>()
                {
                    new Employee()
                    {
                        Id = 1,
                        FirstName = "Test",
                        LastName = "User"
                    }
                });

            _dropdownService = new DropdownService(_employeeRepository.Object);
        }

        [Test]
        public async Task GetEmployeesSuccess()
        {
            var employees = await _dropdownService.GetEmployees();

            Assert.IsNotNull(employees);
            Assert.AreEqual(1, employees.Count);

            var employee = employees[0];
            Assert.AreEqual(1, employee.Id);
            Assert.AreEqual("Test", employee.FirstName);
            Assert.AreEqual("User", employee.LastName);
            Assert.AreEqual("Test User", employee.FullName);
        }
    }
}
