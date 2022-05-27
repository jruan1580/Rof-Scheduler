using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Entities;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Service
{
    [TestFixture]
    public class EmployeeServiceTest
    {
        private Mock<IEmployeeRepository> _employeeRepository;
        private IPasswordService _passwordService;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _employeeRepository = new Mock<IEmployeeRepository>();

            _config = new Mock<IConfiguration>();
            _config.Setup(c => c.GetSection(It.Is<string>(r => r.Equals("Roles"))).Value)
                .Returns("Administrator,Employee");
            _config.Setup(c => c.GetSection(It.Is<string>(r => r.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");
            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public async Task GetAllEmployees_NoEmployees()
        {
            _employeeRepository.Setup(e => e.GetAllEmployees(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Employee>());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            var employees = await employeeService.GetAllEmployees(1, 10);

            Assert.IsEmpty(employees);
        }

        [Test]
        public async Task GetAllEmployees_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetAllEmployees(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Employee>()
                {
                    new Employee()
                    {
                        Id = 1,
                        CountryId = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        Ssn = "123-45-6789",
                        Username = "jdoe",
                        Password = encryptedPass,
                        Role = "Employee",
                        IsLocked = false,
                        FailedLoginAttempts = 0,
                        TempPasswordChanged = false,
                        Status = false,
                        Active = true
                    }
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            var employees = await employeeService.GetAllEmployees(1, 10);

            Assert.IsNotEmpty(employees);
            Assert.AreEqual("John", employees[0].FirstName);
            Assert.AreEqual("Doe", employees[0].LastName);
            Assert.AreEqual("123-45-6789", employees[0].GetNotSanitizedSSN());
            Assert.AreEqual("***-**-6789", employees[0].Ssn);
            Assert.AreEqual("Employee", employees[0].Role);
            Assert.AreEqual("jdoe", employees[0].Username);
            Assert.IsTrue(employees[0].Active);
        }

        [Test]
        public void GetEmployeeById_EmployeeDoesNotExist()
        {
            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync((Employee)null);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);
            
            Assert.ThrowsAsync<EmployeeNotFoundException>(() => employeeService.GetEmployeeById(2));
        }

        [Test]
        public async Task GetEmployeeById_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee() 
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            var employee = await employeeService.GetEmployeeById(1);

            Assert.IsNotNull(employee);
            Assert.AreEqual("John", employee.FirstName);
            Assert.AreEqual("Doe", employee.LastName);
            Assert.AreEqual("123-45-6789", employee.GetNotSanitizedSSN());
            Assert.AreEqual("***-**-6789", employee.Ssn);
            Assert.AreEqual("Employee", employee.Role);
            Assert.AreEqual("jdoe", employee.Username);
            Assert.IsTrue(employee.Active);
            Assert.IsFalse(employee.IsLocked);
            Assert.AreEqual(0, employee.FailedLoginAttempts);
        }

        [Test]
        public async Task GetEmployeeByUsername_EmployeeDoesNotExist()
        {
            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync((Employee)null);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            var employee = await employeeService.GetEmployeeByUsername("jdoe");

            Assert.IsNull(employee);
        }

        [Test]
        public async Task GetEmployeeByUsername_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            var employee = await employeeService.GetEmployeeByUsername("jdoe");

            Assert.IsNotNull(employee);
            Assert.AreEqual("John", employee.FirstName);
            Assert.AreEqual("Doe", employee.LastName);
            Assert.AreEqual("123-45-6789", employee.GetNotSanitizedSSN());
            Assert.AreEqual("***-**-6789", employee.Ssn);
            Assert.AreEqual("Employee", employee.Role);
            Assert.AreEqual("jdoe", employee.Username);
            Assert.AreEqual(encryptedPass, employee.Password);
            Assert.IsTrue(employee.Active);
            Assert.IsFalse(employee.IsLocked);
            Assert.AreEqual(0, employee.FailedLoginAttempts);
            Assert.IsFalse(employee.TempPasswordChanged);
            Assert.IsFalse(employee.Status);
        }        

        [Test]
        public void ResetEmployeeFailedLoginAttempt_EmployeeDoesNotExist()
        {
            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync((Employee)null);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<EmployeeNotFoundException>(() => employeeService.ResetEmployeeFailedLoginAttempt(0));
        }

        [Test]
        public async Task ResetEmployeeFailedLoginAttempt_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = true,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true,
                    AddressLine1 = "123 Abc St",
                    AddressLine2 = "",
                    City = "Oakland",
                    State = "CA",
                    ZipCode = "12345"
                });
                        
            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.ResetEmployeeFailedLoginAttempt(1);

            _employeeRepository.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public void UpdateEmployeeActiveStatus_EmployeeNotFound()
        {
            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync((Employee)null);
        
            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<EmployeeNotFoundException>(() => employeeService.UpdateEmployeeActiveStatus(0, It.IsAny<bool>()));
        }

        [Test]
        public async Task UpdateEmployeeActiveStatus_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 0,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true,
                    AddressLine1 = "123 Abc St", 
                    AddressLine2 = "", 
                    City = "Oakland", 
                    State = "CA", 
                    ZipCode = "12345"
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.UpdateEmployeeActiveStatus(1, false);

            _employeeRepository.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public void UpdateEmployeeInformation_InvalidInput()
        {
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

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdateEmployeeInformation(employee));
        }

        [Test]
        public void UpdateEmployeeInformation_InvalidRole()
        {
            var employee = new Domain.Models.Employee()
            {
                Id = 1,
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                Ssn = "123-45-6789",
                Username = "jdoe",
                Role = "Guest"
            };

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdateEmployeeInformation(employee));
        }

        [Test]
        public async Task UpdateEmployeeInformation_Success()
        {
            var employee = new Domain.Models.Employee()
            {
                Id = 1,
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                Ssn = "123-45-6789",
                Username = "jdoe",
                Role = "Employee",
                Address = new Domain.Models.Address {AddressLine1 = "123 Abc St", AddressLine2 = "", City = "Oakland", State = "CA", ZipCode = "12345"}
            };

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1                   
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.UpdateEmployeeInformation(employee);

            _employeeRepository.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public void CreateEmployee_InvalidInput()
        {
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

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(newEmployee, "tE$t1234"));
        }

        [Test]
        public void CreateEmployee_UsernameNotUnique()
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

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync(new Employee() { Username = "jdoe" });

            _employeeRepository.Setup(e => e.DoesEmployeeExistsBySsnOrUsername(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(true);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(newEmployee, "tE$t1234"));
        }

        [Test]
        public void CreateEmployee_PasswordRequirementNotMet()
        {
            var newEmployee = new Domain.Models.Employee()
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "jdoe",
                Ssn = "123-45-6789",
                Password = null,
                Role = "Employee",
                Active = true
            };

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync(new Employee());

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.CreateEmployee(newEmployee, "abc123"));
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

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.CreateEmployee(newEmployee, "tE$t1234");

            _employeeRepository.Verify(e => e.CreateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public async Task EmployeeLogIn_AlreadyLoggedIn()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = true,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            var employee = await employeeService.EmployeeLogIn("jdoe", "t3$T1234");

            Assert.IsTrue(employee.Status);
        }

        [Test]
        public void EmployeeLogIn_PasswordDoesNotMatch()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                });

            _employeeRepository.Setup(e => e.IncrementEmployeeFailedLoginAttempt(It.IsAny<long>()))
                .ReturnsAsync(2);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.EmployeeLogIn("jdoe", "tE$t1234"));
        }

        [Test]
        public async Task EmployeeLogIn_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.EmployeeLogIn("jdoe", "t3$T1234");

            _employeeRepository.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public async Task EmployeeLogOut_AlreadyLoggedOut()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = false,
                    Active = true
                });

            _employeeRepository.Setup(e => e.UpdateEmployee(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.EmployeeLogout(1);

            _employeeRepository.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Never);
        }

        [Test]
        public async Task EmployeeLogOut_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = true,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.EmployeeLogout(1);

            _employeeRepository.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public void UpdatePassword_RequirementsNotMet()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = true,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdatePassword(1, "abcdefg"));
        }

        [Test]
        public void UpdatePassword_PasswordNotNew()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = true,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            Assert.ThrowsAsync<ArgumentException>(() => employeeService.UpdatePassword(1, "t3$T1234"));
        }

        [Test]
        public async Task UpdatePassword_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = true,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.UpdatePassword(1, "tE$t1234");

            _employeeRepository.Verify(e => e.UpdateEmployee(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public void DeleteEmployeeById_NoEmployee()
        {
            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync((Employee)null);
          
            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            _employeeRepository.Verify(e => e.DeleteEmployeeById(It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task DeleteEmployeeById_Success()
        {
            _employeeRepository.Setup(e => e.DeleteEmployeeById(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            var encryptedPass = _passwordService.EncryptPassword("t3$T1234");

            _employeeRepository.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(new Employee()
                {
                    Id = 1,
                    CountryId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Ssn = "123-45-6789",
                    Username = "jdoe",
                    Password = encryptedPass,
                    Role = "Employee",
                    IsLocked = false,
                    FailedLoginAttempts = 3,
                    TempPasswordChanged = false,
                    Status = true,
                    Active = true
                });

            var employeeService = new EmployeeService(_employeeRepository.Object, _passwordService, _config.Object);

            await employeeService.DeleteEmployeeById(1);

            _employeeRepository.Verify(e => e.DeleteEmployeeById(It.IsAny<long>()), Times.Once);
        }
    }
}
