﻿using EmployeeManagementService.Domain.Mappers.Database;
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

            var roles = _roles.Split(",");

            if (!roles.Contains(employee.Role))
            {
                throw new ArgumentException("Invalid role assigned");
            }

            await _employeeRepository.UpdateEmployeeInformation(employee.Id, employee.Username, employee.FirstName, employee.LastName, employee.Role, employee.Ssn);
        }

        public async Task CreateEmployee(Employee newEmployee, string password)
        {
            var invalidErrors = newEmployee.IsValidEmployeeToCreate().ToArray();

            if (invalidErrors.Length > 0)
            {
                var errorMessage = string.Join("\n", invalidErrors);

                throw new ArgumentException(errorMessage);
            }

            byte[] encryptedPass = null;

            if (_passwordService.VerifyPasswordRequirements(password))
            {
                encryptedPass = _passwordService.EncryptPassword(password);
            }

            var roles = _roles.Split(",");

            if(!roles.Contains(newEmployee.Role))
            {
                throw new ArgumentException("Invalid role assigned");
            }

            await _employeeRepository.CreateEmployee(newEmployee.FirstName, newEmployee.LastName, newEmployee.Username, newEmployee.Ssn, encryptedPass, newEmployee.Role, newEmployee.Active);
        }

        public async Task EmployeeLogIn(string username, string password)
        {
            //get employee by username
            var employee = new Employee();

            if(!_passwordService.VerifyPasswordHash(password, employee.Password))
            {
                throw new ArgumentException("Incorrect password");
            }

            if(employee.Status == true)
            {
                return;
            }

            await _employeeRepository.UpdateEmployeeLoginStatus(employee.Id, true);
        }

        public async Task EmployeeLogout(long id)
        {
            var employee = await GetEmployeeById(id);

            if (employee.Status == false)
            {
                return;
            }

            await _employeeRepository.UpdateEmployeeLoginStatus(employee.Id, false);
        }
    }
}
