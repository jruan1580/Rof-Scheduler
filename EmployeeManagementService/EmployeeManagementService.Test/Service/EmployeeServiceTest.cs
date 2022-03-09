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

        [Test]
        public void UpdateEmployeeInformation_InvalidInput()
        {
            _employeeRepository.Setup(e => e.UpdateEmployeeInformation(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdateEmployeeInformation(new Domain.Models.Employee()));
        }

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            _employeeRepository.Setup(e => e.GetEmployeeById(It.IsAny<long>()))
                .ReturnsAsync(new Employee());

            _employeeRepository.Setup(e => e.UpdateEmployeeInformation(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.UpdateEmployeeInformation(new Domain.Models.Employee() { Id = 1, Username = "jdoe", FirstName = "John", LastName = "Doe", Role = "Employee", Ssn = "123-45-6789"});

            _employeeRepository.Verify(e => e.UpdateEmployeeInformation(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CreateEmployee_InvalidInput()
        {
            _employeeRepository.Setup(e => e.CreateEmployee(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(new Domain.Models.Employee(), "tE$t1234"));
        }

        [Test]
        public void CreateEmployee_UsernameNotUnique()
        {
            var newEmployee = new Employee()
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "jdoe",
                Ssn = "123-45-6789",
                Password = new byte[32],
                Role = "Employee",
                Active = true
            };

            _employeeRepository.Setup(e => e.GetEmployeeByUsername(It.IsAny<string>()))
                .ReturnsAsync(new Employee() { Username = "Jdoe" });

            _employeeRepository.Setup(e => e.CreateEmployee(It.Is<string>(f => f.Equals(newEmployee.FirstName)), It.Is<string>(l => l.Equals(newEmployee.LastName)), It.Is<string>(u => u.Equals(newEmployee.Username)), 
                It.Is<string>(s => s.Equals(newEmployee.Ssn)), It.Is<byte[]>(p => p.Equals(newEmployee.Password)), It.Is<string>(r => r.Equals(newEmployee.Role)), It.Is<bool?>(a => a.Equals(newEmployee.Active))))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(new Domain.Models.Employee(), "tE$t1234"));
        }

        [Test]
        public void CreateEmployee_PasswordRequirementNotMet()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.Is<string>(p => p.Equals("tE$t1"))))
                .Returns(false);

            _employeeRepository.Setup(e => e.CreateEmployee(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(u => u.Equals("Jdoe")), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(new Domain.Models.Employee(), "abc123"));
        }

        [Test]
        public void CreateEmployee_PasswordEncryptionFail()
        {
            _passwordService.Setup(p => p.EncryptPassword(It.Is<string>(p => p.Equals("tE$t1234"))))
                .Throws(new Exception());

            _employeeRepository.Setup(e => e.CreateEmployee(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(u => u.Equals("Jdoe")), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .ThrowsAsync(new ArgumentException());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(new Domain.Models.Employee(), "tE$t1234"));
        }

        [Test]
        public async Task CreateEmployee_Success()
        {
            var newEmployee = new Domain.Models.Employee()
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "jdoe",
                Ssn = "123-45-6789",
                Password = new byte[32],
                Role = "Employee",
                Active = true
            };

            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.Is<string>(p => p.Equals("tE$t1234"))))
                .Returns(true);

            _passwordService.Setup(p => p.EncryptPassword(It.Is<string>(p => p.Equals("tE$t1234"))))
                .Returns(new byte[32]);

            _employeeRepository.Setup(e => e.CreateEmployee(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(u => u.Equals("Jdoe")), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<bool?>()))
                .Returns(Task.CompletedTask);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService.Object, _config.Object);

            await employeeService.CreateEmployee(newEmployee, "tE$t1234");

            _employeeRepository.Verify(e => e.CreateEmployee(It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(u => u.Equals("Jdoe")), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<bool?>()), Times.Once);
        }
    }
}
