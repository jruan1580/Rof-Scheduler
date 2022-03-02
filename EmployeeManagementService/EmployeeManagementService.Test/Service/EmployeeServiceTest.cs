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
            _config.Setup(c => c.GetSection(It.Is<string>(r => r.Equals("Roles"))).Value)
                .Returns("Administrator,Employee");
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

        [Test]
        public void GetEmployeeById_EmployeeDoesNotExist()
        {
            _employeeRepository.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.GetEmployeeById(0));
        }

        [Test]
        public async Task GetEmployeeById_Success()
        {
            _employeeRepository.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(new Employee());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.GetEmployeeById(1);

            _employeeRepository.Verify(e => e.GetEmployeeById(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public void GetEmployeeByUsername_EmployeeDoesNotExist()
        {
            _employeeRepository.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.GetEmployeeByUsername("JDoe"));
        }

        [Test]
        public async Task GetEmployeeByUsername_Success()
        {
            _employeeRepository.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ReturnsAsync(new Employee());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.GetEmployeeByUsername("JDoe");

            _employeeRepository.Verify(e => e.GetEmployeeByUsername(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IncrementEmployeeFailedLoginAttempt_EmployeeDoesNotExist()
        {
            _employeeRepository.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.IncrementEmployeeFailedLoginAttempt(0));
        }

        [Test]
        public async Task IncrementEmployeeFailedLoginAttempt_Success()
        {
            _employeeRepository.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(new Employee());

            _employeeRepository.Setup(e => e.IncrementEmployeeFailedLoginAttempt(It.IsAny<long>()))
                .ReturnsAsync(1);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.IncrementEmployeeFailedLoginAttempt(1);

            _employeeRepository.Verify(e => e.IncrementEmployeeFailedLoginAttempt(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public void ResetEmployeeFailedLoginAttempt_EmployeeDoesNotExist()
        {
            _employeeRepository.Setup(e => e.ResetEmployeeFailedLoginAttempt(It.IsAny<long>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.ResetEmployeeFailedLoginAttempt(0));
        }

        [Test]
        public async Task ResetEmployeeFailedLoginAttempt_Success()
        {
            _employeeRepository.Setup(e => e.ResetEmployeeFailedLoginAttempt(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            _employeeRepository.Setup(e => e.UpdateEmployeeIsLockedStatus(It.IsAny<long>(), It.Is<bool>(l => l.Equals(false))))
                .Returns(Task.CompletedTask);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.ResetEmployeeFailedLoginAttempt(1);

            _employeeRepository.Verify(e => e.ResetEmployeeFailedLoginAttempt(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public void UpdateEmployeeActiveStatus_EmployeeNotFound()
        {
            _employeeRepository.Setup(e => e.UpdateEmployeeActiveStatus(It.IsAny<long>(), It.IsAny<bool>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdateEmployeeActiveStatus(0, It.IsAny<bool>()));
        }

        [Test]
        public async Task UpdateEmployeeActiveStatus_Success()
        {
            _employeeRepository.Setup(e => e.UpdateEmployeeActiveStatus(It.IsAny<long>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.UpdateEmployeeActiveStatus(1, false);

            _employeeRepository.Verify(e => e.UpdateEmployeeActiveStatus(It.IsAny<long>(), It.IsAny<bool>()), Times.Once);
        }
    }
}
