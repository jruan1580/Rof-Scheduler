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
    public class EmployeeService
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

            if(employees == null || employees.Count == 0)
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

            await _employeeRepository.UpdateEmployeeInformation(employee.Id, employee.Username, employee.FirstName, employee.LastName, employee.Role, employee.Ssn);
        }

        public async Task CreateEmployee(string firstName, string lastName, string username, string ssn, string password, string role, bool active)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("First name cannot be empty");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("Last name cannot be empty");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be empty");
            }

            if (string.IsNullOrEmpty(ssn))
            {
                throw new ArgumentException("SSN cannot be empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentException("Role cannot be empty");
            }

            byte[] encryptedPass = null;

            if (_passwordService.VerifyPasswordRequirements(password))
            {
                encryptedPass = _passwordService.EncryptPassword(password);
            }

            var roles = _roles.Split(", ");

            if(!roles.Contains(role))
            {
                throw new ArgumentException("Invalid role assigned");
            }

            await _employeeRepository.CreateEmployee(firstName, lastName, username, ssn, encryptedPass, role, active);
        }
    }
}
