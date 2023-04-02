using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Entities;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Moq;
using NUnit.Framework;
using RofShared.Exceptions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementService.Test.Service
{
    [TestFixture]
    public class EmployeeRetrievalServiceTests
    {
        [Test]
        public async Task GetAllEmployees_NoEmployees()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();

            employeeRetrievalRepo.Setup(e => 
                e.GetAllEmployeesByKeyword(
                    It.IsAny<int>(), 
                    It.IsAny<int>(), 
                    It.IsAny<string>()))
            .ReturnsAsync((new List<Employee>(), 0));

            var employeeService = new EmployeeRetrievalService(employeeRetrievalRepo.Object);

            var result = await employeeService.GetAllEmployeesByKeyword(1, 10, "");

            Assert.IsEmpty(result.Employees);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public async Task GetAllEmployees_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();

            var employees = new List<Employee>()
            {
                EmployeeCreator.GetDbEmployee(Encoding.UTF8.GetBytes("password"))
            };

            employeeRetrievalRepo.Setup(e => e.GetAllEmployeesByKeyword(
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<string>()))
            .ReturnsAsync((employees, 1));

            var employeeService = new EmployeeRetrievalService(employeeRetrievalRepo.Object);

            var result = await employeeService.GetAllEmployeesByKeyword(1, 10, "");

            Assert.IsNotEmpty(result.Employees);
            Assert.AreEqual("John", result.Employees[0].FirstName);
            Assert.AreEqual("Doe", result.Employees[0].LastName);
            Assert.AreEqual("123-45-6789", result.Employees[0].Ssn);
            Assert.AreEqual("Employee", result.Employees[0].Role);
            Assert.AreEqual("jdoe", result.Employees[0].Username);
        }

        [Test]
        public void GetEmployeeById_EmployeeDoesNotExist()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
            .ReturnsAsync((Employee)null);

            var employeeService = new EmployeeRetrievalService(employeeRetrievalRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => employeeService.GetEmployeeById(2));
        }

        [Test]
        public async Task GetEmployeeById_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();

            employeeRetrievalRepo.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<long>>()))
                .ReturnsAsync(EmployeeCreator.GetDbEmployee(Encoding.UTF8.GetBytes("password")));

            var employeeService = new EmployeeRetrievalService(employeeRetrievalRepo.Object);

            var employee = await employeeService.GetEmployeeById(1);

            Assert.IsNotNull(employee);
            Assert.AreEqual("John", employee.FirstName);
            Assert.AreEqual("Doe", employee.LastName);
            Assert.AreEqual("123-45-6789", employee.Ssn);
            Assert.AreEqual("Employee", employee.Role);
            Assert.AreEqual("jdoe", employee.Username);
        }

        [Test]
        public void GetEmployeeByUsername_EmployeeDoesNotExist()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();

            employeeRetrievalRepo.Setup(e => 
                e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
            .ThrowsAsync(new EntityNotFoundException("Employee"));

            var employeeService = new EmployeeRetrievalService(employeeRetrievalRepo.Object);

            Assert.ThrowsAsync<EntityNotFoundException>(() => employeeService.GetEmployeeByUsername("jdoe"));
        }

        [Test]
        public async Task GetEmployeeByUsername_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();

            employeeRetrievalRepo.Setup(e => e.GetEmployeeByFilter(It.IsAny<GetEmployeeFilterModel<string>>()))
                .ReturnsAsync(EmployeeCreator.GetDbEmployee(Encoding.UTF8.GetBytes("password")));

            var employeeService = new EmployeeRetrievalService(employeeRetrievalRepo.Object);

            var employee = await employeeService.GetEmployeeByUsername("jdoe");

            Assert.IsNotNull(employee);
            Assert.AreEqual("John", employee.FirstName);
            Assert.AreEqual("Doe", employee.LastName);
            Assert.AreEqual("123-45-6789", employee.Ssn);
            Assert.AreEqual("Employee", employee.Role);
            Assert.AreEqual("jdoe", employee.Username);
            Assert.IsFalse(employee.TempPasswordChanged);
        }

        [Test]
        public async Task GetEmployeesForDropdown_Success()
        {
            var employeeRetrievalRepo = new Mock<IEmployeeRetrievalRepository>();

            employeeRetrievalRepo.Setup(e => e.GetEmployeesForDropdown())
                .ReturnsAsync(new List<Employee>()
                {
                    new Employee()
                    {
                        Id = 1,
                        FirstName = "Test",
                        LastName = "User"
                    }
                });

            var employeeService = new EmployeeRetrievalService(employeeRetrievalRepo.Object);

            var result = await employeeService.GetEmployeesForDropdown();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            var employee = result[0];
            Assert.AreEqual(1, employee.Id);
            Assert.AreEqual("Test", employee.FirstName);
            Assert.AreEqual("User", employee.LastName);
            Assert.AreEqual("Test User", employee.FullName);
        }
    }
}
