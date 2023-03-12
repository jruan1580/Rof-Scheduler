using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Entities;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RofShared.Services;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Service
{
    [TestFixture]
    public class EmployeeAuthServiceTests
    {
        private IPasswordService _passwordService;

        private Mock<IConfiguration> _config;

        private readonly string _passwordUnencrypted = "t3$T1234";

        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>();

            _config.Setup(c => 
                c.GetSection(It.Is<string>(r => r.Equals("PasswordSalt"))).Value)
            .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public async Task EmployeeLogIn_AlreadyLoggedIn()
        {
            var employeeRepository = new Mock<IEmployeeRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = true;

            employeeRepository.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRepository.Object, _passwordService);

            var employee = await employeeService.EmployeeLogIn("jdoe", _passwordUnencrypted);

            Assert.IsTrue(employee.Status);
        }

        [Test]
        public void EmployeeLogIn_PasswordDoesNotMatch()
        {
            var employeeRepository = new Mock<IEmployeeRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = false;

            employeeRepository.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            short failedAttempts = 2;
            employeeRepository.Setup(e => e.IncrementEmployeeFailedLoginAttempt(It.IsAny<long>()))
                .ReturnsAsync(failedAttempts);

            var employeeService = new EmployeeAuthService(employeeRepository.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.EmployeeLogIn("jdoe", "tE$t1234"));
        }

        [Test]
        public void EmployeeLogin_Locked()
        {
            var employeeRepository = new Mock<IEmployeeRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = false;
            dbEmployee.IsLocked = true;

            employeeRepository.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRepository.Object, _passwordService);

            Assert.ThrowsAsync<EmployeeIsLockedException>(() => employeeService.EmployeeLogIn("jdoe", _passwordUnencrypted));
        }

        [Test]
        public async Task EmployeeLogIn_Success()
        {
            var employeeRepository = new Mock<IEmployeeRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = false;

            employeeRepository.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRepository.Object, _passwordService);

            await employeeService.EmployeeLogIn("jdoe", _passwordUnencrypted);

            employeeRepository.Verify(e => 
                e.UpdateEmployee(It.Is<Employee>(e => e.Status == true)), 
            Times.Once);
        }
       
        [Test]
        public async Task EmployeeLogOut_Success()
        {
            var employeeRepository = new Mock<IEmployeeRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = true;

            employeeRepository.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRepository.Object, _passwordService);

            await employeeService.EmployeeLogout(1);

            employeeRepository.Verify(e => e.UpdateEmployee(It.Is<Employee>(e => !e.Status)), Times.Once);
        }       
    }
}
