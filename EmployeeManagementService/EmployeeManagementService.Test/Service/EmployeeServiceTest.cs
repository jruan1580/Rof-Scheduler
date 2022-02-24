using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Service
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private Mock<IEmployeeRepository> _employeeRepository;
        private Mock<IPasswordService> _passwordService;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _employeeRepository = new Mock<IEmployeeRepository>();
            _passwordService = new Mock<IPasswordService>();
            _config = new Mock<IConfiguration>();
        }

        [Test]
        public void GetAllEmployees_ExtraPagesReturned()
        {
            _employeeRepository.Setup(e => e.GetAllEmployees(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.GetAllEmployees(1, 10));
        }

        [Test]
        public async Task GetAllEmployees_Success()
        {
            _employeeRepository.Setup(e => e.GetAllEmployees(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Employee>());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.GetAllEmployees(1, 10);

            _employeeRepository.Verify(e => e.GetAllEmployees(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}
