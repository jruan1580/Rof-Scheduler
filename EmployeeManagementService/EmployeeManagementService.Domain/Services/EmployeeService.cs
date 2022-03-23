using EmployeeManagementService.Domain.Mappers.Database;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public interface IEmployeeService
    {
        Task CreateEmployee(Employee newEmployee, string password);
        Task EmployeeLogIn(string username, string password);
        Task EmployeeLogout(long id);
        Task<List<Employee>> GetAllEmployees(int page, int offset);
        Task<Employee> GetEmployeeById(long id);
        Task<Employee> GetEmployeeByUsername(string username);
        Task IncrementEmployeeFailedLoginAttempt(long id);
        Task ResetEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployeeActiveStatus(long id, bool active);
        Task UpdateEmployeeInformation(Employee employee);
        Task UpdatePassword(long id, string newPassword);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPasswordService _passwordService;
        private readonly string _roles;

        public EmployeeService(IEmployeeRepository employeeRepository, IPasswordService passwordService, IConfiguration config)
        {
            _employeeRepository = employeeRepository;
            _passwordService = passwordService;
            _roles = config.GetSection("Roles").Value;
        }

        public async Task<List<Employee>> GetAllEmployees(int page, int offset)
        {
            var employees = await _employeeRepository.GetAllEmployees(page, offset);

            if (employees == null || employees.Count == 0)
            {
                return new List<Employee>();
            }

            return employees.Select(e => EmployeeMapper.ToCoreEmployee(e)).ToList();
        }

        public async Task<Employee> GetEmployeeById(long id)
        {
            var employee = await _employeeRepository.GetEmployeeById(id);

            if (employee == null)
            {
                throw new ArgumentException("Employee does not exist.");
            }

            return EmployeeMapper.ToCoreEmployee(employee);
        }

        public async Task<Employee> GetEmployeeByUsername(string username)
        {
            var employee = await _employeeRepository.GetEmployeeByUsername(username);

            if (employee == null)
            {
                throw new ArgumentException("Employee does not exist.");
            }

            return EmployeeMapper.ToCoreEmployee(employee);
        }

        public async Task IncrementEmployeeFailedLoginAttempt(long id)
        {
            var employee = await GetEmployeeById(id);

            if (employee.IsLocked)
            {
                return;
            }

            var attempts = await _employeeRepository.IncrementEmployeeFailedLoginAttempt(id);

            if (attempts != 3)
            {
                return;
            }

            await _employeeRepository.UpdateEmployeeIsLockedStatus(id, true);
        }

        public async Task ResetEmployeeFailedLoginAttempt(long id)
        {
            await _employeeRepository.ResetEmployeeFailedLoginAttempt(id);

            await _employeeRepository.UpdateEmployeeIsLockedStatus(id, false);
        }

        public async Task UpdateEmployeeActiveStatus(long id, bool active)
        {
            await _employeeRepository.UpdateEmployeeActiveStatus(id, active);
        }

        public async Task UpdateEmployeeInformation(Employee employee)
        {
            var invalidErrors = employee.IsValidEmployeeForUpdate().ToArray();

            if (invalidErrors.Length > 0)
            {
                var errorMessage = string.Join("\n", invalidErrors);

                throw new ArgumentException(errorMessage);
            }

            var roles = _roles.Split(",");

            if (!roles.Contains(employee.Role))
            {
                throw new ArgumentException("Invalid role assigned");
            }

            await _employeeRepository.UpdateEmployeeInformation(employee.Id, employee.Username, employee.FirstName, employee.LastName, employee.Role, employee.Ssn, 
                employee.Address.AddressLine1, employee.Address.AddressLine2, employee.Address.City, employee.Address.State, employee.Address.ZipCode);
        }

        public async Task CreateEmployee(Employee newEmployee, string password)
        {
            var invalidErrors = newEmployee.IsValidEmployeeToCreate().ToArray();

            if (invalidErrors.Length > 0)
            {
                var errorMessage = string.Join("\n", invalidErrors);

                throw new ArgumentException(errorMessage);
            }

            var employeeCheck = await GetEmployeeByUsername(newEmployee.Username);

            if (newEmployee.Username == employeeCheck.Username)
            {
                throw new ArgumentException("Username already exists");
            }

            if (!_passwordService.VerifyPasswordRequirements(password))
            {
                throw new ArgumentException("Password does not meet requirements");
            }

            byte[] encryptedPass = null;

            encryptedPass = _passwordService.EncryptPassword(password);

            var roles = _roles.Split(",");

            if (!roles.Contains(newEmployee.Role))
            {
                throw new ArgumentException("Invalid role assigned");
            }

            await _employeeRepository.CreateEmployee(newEmployee.FirstName, newEmployee.LastName, newEmployee.Username, newEmployee.Ssn, encryptedPass, newEmployee.Role,
                newEmployee.Address.AddressLine1, newEmployee.Address.AddressLine2, newEmployee.Address.City, newEmployee.Address.State, newEmployee.Address.ZipCode, newEmployee.Active);
        }

        public async Task EmployeeLogIn(string username, string password)
        {
            var employee = await GetEmployeeByUsername(username);

            if (!_passwordService.VerifyPasswordHash(password, employee.Password))
            {
                throw new ArgumentException("Incorrect password");
            }

            if (employee.Status == true)
            {
                throw new ArgumentException("Employee already logged in");
            }

            await _employeeRepository.UpdateEmployeeLoginStatus(employee.Id, true);
        }

        public async Task EmployeeLogout(long id)
        {
            var employee = await GetEmployeeById(id);

            if (employee.Status == false)
            {
                throw new ArgumentException("Employee already logged out");
            }

            await _employeeRepository.UpdateEmployeeLoginStatus(employee.Id, false);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var employee = await GetEmployeeById(id);

            if (!_passwordService.VerifyPasswordRequirements(newPassword))
            {
                throw new ArgumentException("New password does not meet all requirements.");
            }

            if (_passwordService.VerifyPasswordHash(newPassword, employee.Password))
            {
                throw new ArgumentException("New password cannot be the same as current password.");
            }

            var newEncryptedPass = _passwordService.EncryptPassword(newPassword);

            await _employeeRepository.UpdatePassword(employee.Id, newEncryptedPass);
        }
    }
}
