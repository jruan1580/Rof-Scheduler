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
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = true;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRetrievalRepo.Object, employeeUpsertRepo.Object, _passwordService);

            var employee = await employeeService.EmployeeLogIn("jdoe", _passwordUnencrypted);

            Assert.IsTrue(employee.Status);
        }

        [Test]
        public void EmployeeLogIn_PasswordDoesNotMatch()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = false;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            short failedAttempts = 2;
            employeeUpsertRepo.Setup(e => e.IncrementEmployeeFailedLoginAttempt(It.IsAny<long>()))
                .ReturnsAsync(failedAttempts);

            var employeeService = new EmployeeAuthService(employeeRetrievalRepo.Object, employeeUpsertRepo.Object, _passwordService);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.EmployeeLogIn("jdoe", "tE$t1234"));
        }

        [Test]
        public void EmployeeLogin_Locked()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = false;
            dbEmployee.IsLocked = true;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRetrievalRepo.Object, employeeUpsertRepo.Object, _passwordService);

            Assert.ThrowsAsync<EmployeeIsLockedException>(() => employeeService.EmployeeLogIn("jdoe", _passwordUnencrypted));
        }

        [Test]
        public async Task EmployeeLogIn_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = false;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRetrievalRepo.Object, employeeUpsertRepo.Object, _passwordService);

            await employeeService.EmployeeLogIn("jdoe", _passwordUnencrypted);

            employeeUpsertRepo.Verify(e => 
                e.UpdateEmployee(It.Is<Employee>(e => e.Status == true)), 
            Times.Once);
        }
       
        [Test]
        public async Task EmployeeLogOut_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var dbEmployee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            dbEmployee.Status = true;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(dbEmployee);

            var employeeService = new EmployeeAuthService(employeeRetrievalRepo.Object, employeeUpsertRepo.Object, _passwordService);

            await employeeService.EmployeeLogout(1);

            employeeUpsertRepo.Verify(e => e.UpdateEmployee(It.Is<Employee>(e => !e.Status)), Times.Once);
        }       
    }
}
