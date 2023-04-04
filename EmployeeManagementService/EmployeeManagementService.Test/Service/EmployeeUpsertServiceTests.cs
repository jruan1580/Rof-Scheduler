using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Entities;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using RofShared.Services;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Service
{
    [TestFixture]
    public class EmployeeUpsertServiceTests
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

            _config.Setup(c => 
                c.GetSection(It.Is<string>(r => r.Equals("Roles"))).Value)
            .Returns("Administrator,Employee");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public void ResetEmployeeFailedLoginAttempt_EmployeeDoesNotExist()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync((Employee)null);

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object, 
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => employeeService.ResetEmployeeFailedLoginAttempt(0));
        }

        [Test]
        public async Task ResetEmployeeFailedLoginAttempt_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var employee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            employee.IsLocked = true;
            employee.FailedLoginAttempts = 3;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(employee);

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            await employeeService.ResetEmployeeFailedLoginAttempt(1);

            employeeUpsertRepo.Verify(e => 
                e.UpdateEmployee(It.Is<Employee>(e => 
                    !e.IsLocked &&
                    e.FailedLoginAttempts == 0)),
             Times.Once);
        }

        [Test]
        public void UpdateEmployeeActiveStatus_EmployeeNotFound()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync((Employee)null);

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => employeeService.UpdateEmployeeActiveStatus(0, It.IsAny<bool>()));
        }

        [Test]
        public async Task UpdateEmployeeActiveStatus_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var employee = EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            employee.Active = false;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(employee);

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            await employeeService.UpdateEmployeeActiveStatus(1, true);

            employeeUpsertRepo.Verify(e => 
                e.UpdateEmployee(It.Is<Employee>(e => e.Active == true)), 
             Times.Once);
        }

        [Test]
        public void UpdateEmployeeInformation_InvalidInput()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var employee = new Domain.Models.Employee()
            {
                Id = 0,
                CountryId = 1,
                FirstName = "",
                LastName = "",
                Ssn = "",
                Username = "",
                Role = ""
            };

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdateEmployeeInformation(employee));
        }

        [Test]
        public void UpdateEmployeeInformation_InvalidRole()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var employee = EmployeeCreator.GetDomainEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            employee.Role = "Guest";

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdateEmployeeInformation(employee));
        }

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var employee = EmployeeCreator.GetDomainEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            employee.Id = 1;

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted)));

            employeeRetrievalRepo.Setup(e =>
               e.DoesEmployeeExistsBySsnOrUsernameOrEmail(It.IsAny<string>(),
                   It.IsAny<string>(),
                   It.IsAny<string>(),
                   It.IsAny<long>()))
           .ReturnsAsync(false);

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            await employeeService.UpdateEmployeeInformation(employee);

            employeeUpsertRepo.Verify(e => 
                e.UpdateEmployee(It.Is<Employee>(e => e.Id == employee.Id &&
                    e.FirstName == employee.FirstName &&
                    e.LastName == employee.LastName &&
                    e.Ssn == employee.Ssn &&
                    e.EmailAddress == employee.Email &&
                    e.Role == employee.Role)), 
            Times.Once);
        }

        [Test]
        public void CreateEmployee_InvalidInput()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var newEmployee = new Domain.Models.Employee()
            {
                FirstName = "",
                LastName = "",
                Username = "",
                Ssn = "",
                Password = null,
                Role = "",
                Active = true
            };

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(newEmployee, _passwordUnencrypted));
        }

        [Test]
        public void CreateEmployee_UsernameNotUnique()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var newEmployee = EmployeeCreator.GetDomainEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            newEmployee.Password = null;

            employeeRetrievalRepo.Setup(e => 
                e.DoesEmployeeExistsBySsnOrUsernameOrEmail(It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<long>()))
            .ReturnsAsync(true);

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(newEmployee, _passwordUnencrypted));
        }

        [Test]
        public void CreateEmployee_PasswordRequirementNotMet()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var newEmployee = EmployeeCreator.GetDomainEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            newEmployee.Password = null;

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(newEmployee, "abc123"));
        }

        [Test]
        public async Task CreateEmployee_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            var newEmployee = EmployeeCreator.GetDomainEmployee(_passwordService.EncryptPassword(_passwordUnencrypted));
            newEmployee.Password = null;

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            await employeeService.CreateEmployee(newEmployee, _passwordUnencrypted);

            employeeUpsertRepo.Verify(e => 
                e.CreateEmployee(It.Is<Employee>(e => e.FirstName == newEmployee.FirstName &&
                    e.LastName == newEmployee.LastName &&
                    e.EmailAddress == newEmployee.Email &&
                    e.Ssn == newEmployee.Ssn &&
                    e.Role == newEmployee.Role)), 
            Times.Once);
        }

        [Test]
        public void UpdatePassword_RequirementsNotMet()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdatePassword(1, "abcdefg"));
        }

        [Test]
        public void UpdatePassword_PasswordNotNew()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            employeeRetrievalRepo.Setup(e =>
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdatePassword(1, _passwordUnencrypted));
        }

        [Test]
        public async Task UpdatePassword_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            employeeRetrievalRepo.Setup(e =>
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            var newPassword = "tE$t1234";

            await employeeService.UpdatePassword(1, newPassword);

            employeeUpsertRepo.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public void DeleteEmployeeById_NoEmployee()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync((Employee)null);

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                _passwordService,
                _config.Object);

            employeeUpsertRepo.Verify(e => e.DeleteEmployeeById(It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task DeleteEmployeeById_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();
            var employeeUpsertRepo = new Mock<IEmployeeUpsertRepository>();

            employeeUpsertRepo.Setup(e => e.DeleteEmployeeById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            employeeRetrievalRepo.Setup(e =>
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync(EmployeeCreator.GetDbEmployee(_passwordService.EncryptPassword(_passwordUnencrypted)));

            var employeeService = new EmployeeUpsertService(employeeRetrievalRepo.Object,
                employeeUpsertRepo.Object,
                 _passwordService,
                 _config.Object);

            await employeeService.DeleteEmployeeById(1);

            employeeUpsertRepo.Verify(e => e.DeleteEmployeeById(It.Is<long>(id => id == 1)), Times.Once);
        }
    }
}
