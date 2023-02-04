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

        protected async Task ValidateEmployeeInformation(Employee employee, bool isUpdate)
        {
            var invalidErrors = (isUpdate) 
                ? employee.IsValidEmployeeForUpdate().ToArray()
                : employee.IsValidEmployeeToCreate().ToArray();

            if (invalidErrors.Length > 0)
            {
                var errorMessage = string.Join("\n", invalidErrors);

                throw new ArgumentException(errorMessage);
            }

            var isDuplicate = await _employeeRepository.DoesEmployeeExistsBySsnOrUsernameOrEmail(employee.Ssn,
                employee.Username, employee.Email, employee.Id);

            if (isDuplicate)
            {
                throw new ArgumentException("Employee with ssn, username, or email exists");
            }

            var roles = _roles.Split(",");
            if (string.IsNullOrEmpty(employee.Role) || !roles.Contains(employee.Role))
            {
                throw new ArgumentException("Invalid role assigned");
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
    }
}
