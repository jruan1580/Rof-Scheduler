using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Microsoft.Extensions.Configuration;
using RofShared.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using EmployeeDB = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Services
{
    public class BaseEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        protected readonly string _roles;

        public BaseEmployeeService(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;

            _roles = configuration.GetSection("Roles").Value;
        }

        protected async Task<EmployeeDB> GetDbEmployeeById(long id)
        {
            var filterModel = new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id);

            var employee = await _employeeRepository.GetEmployeeByFilter(filterModel);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return employee;
        }

        protected async Task<EmployeeDB> GetDbEmployeeByUsername(string username)
        {
            var filterModel = new GetEmployeeFilterModel<string>(GetEmployeeFilterEnum.Usermame, username);

            var employee = await _employeeRepository.GetEmployeeByFilter(filterModel);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return employee;
        }

        protected async Task ValidateEmployee(Employee employee, bool isUpdate)
        {
            ValidateEmployeeProperties(employee, isUpdate);

            await ValidateIfEmployeeIsDuplicate(employee.Id, employee.Ssn, employee.Username, employee.Email);

            if (!isUpdate)
            {
                ValidateEmployeeRole(employee.Role);
            }            
        }

        protected async Task IncrementEmployeeFailedLoginAttempt(EmployeeDB employee)
        {
            if (employee.IsLocked)
            {
                return;
            }

            var attempts = await _employeeRepository.IncrementEmployeeFailedLoginAttempt(employee.Id);

            if (attempts != 3)
            {
                return;
            }

            employee.IsLocked = true;
            employee.FailedLoginAttempts = attempts;

            await _employeeRepository.UpdateEmployee(employee);
        }

        private void ValidateEmployeeProperties(Employee employee, bool isUpdate)
        {
            var validationErrors = (isUpdate) ? employee.GetValidationErrorsForUpdate() : employee.GetValidationErrorsForCreate();

            if (validationErrors.Count > 0)
            {
                var errorMessage = string.Join("\n", validationErrors);

                throw new ArgumentException(errorMessage);
            }
        }

        private async Task ValidateIfEmployeeIsDuplicate(long id, string ssn, string username, string email)
        {
            var isDuplicate = await _employeeRepository.DoesEmployeeExistsBySsnOrUsernameOrEmail(ssn,
                username, email, id);

            if (isDuplicate)
            {
                throw new ArgumentException("Employee with ssn, username, or email exists");
            }
        }

        private void ValidateEmployeeRole(string role)
        {
            var roles = _roles.Split(",");
            if (string.IsNullOrEmpty(role) || !roles.Contains(role))
            {
                throw new ArgumentException($"Invalid role assigned - {role}");
            }
        }
    }
}
