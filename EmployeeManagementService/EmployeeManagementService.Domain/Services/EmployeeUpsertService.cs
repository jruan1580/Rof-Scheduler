using EmployeeManagementService.Domain.Mappers.Database;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using RofShared.Exceptions;
using RofShared.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Services
{
    public class EmployeeUpsertService : EmployeeService, IEmployeeUpsertService
    {
        private readonly IEmployeeUpsertRepository _employeeUpsertRepository;

        private readonly IPasswordService _passwordService;

        private readonly string _roles;

        public EmployeeUpsertService(IEmployeeRetrievalRepository employeeRetrievalRepository,
            IEmployeeUpsertRepository employeeUpsertRepository,
            IPasswordService passwordService,
            IConfiguration configuration) : base(employeeRetrievalRepository)
        {
            _employeeUpsertRepository = employeeUpsertRepository;

            _passwordService = passwordService;

            _roles = configuration.GetSection("Roles").Value;
        }

        public async Task ResetEmployeeFailedLoginAttempt(long id)
        {
            var employee = await GetDbEmployeeById(id);

            employee.FailedLoginAttempts = 0;
            employee.IsLocked = false;

            await _employeeUpsertRepository.UpdateEmployee(employee);
        }

        public async Task UpdateEmployeeActiveStatus(long id, bool active)
        {
            var employee = await GetDbEmployeeById(id);

            employee.Active = active;

            await _employeeUpsertRepository.UpdateEmployee(employee);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var employee = await GetDbEmployeeById(id);

            _passwordService.ValidateNewPasswordForUpdate(newPassword, employee.Password);

            employee.Password = _passwordService.EncryptPassword(newPassword);

            await _employeeUpsertRepository.UpdateEmployee(employee);
        }

        public async Task UpdateEmployeeInformation(Employee employee)
        {
            await ValidateEmployee(employee, true);

            var originalEmployee = await GetDbEmployeeById(employee.Id);

            MergeEmployeePropertiesForUpdate(originalEmployee, employee);

            await _employeeUpsertRepository.UpdateEmployee(originalEmployee);
        }

        public async Task CreateEmployee(Employee newEmployee, string password)
        {
            await ValidateEmployee(newEmployee, false);

            _passwordService.ValidatePasswordForCreate(password);

            newEmployee.Password = _passwordService.EncryptPassword(password);

            var newEmployeeEntity = EmployeeMapper.FromCoreEmployee(newEmployee);

            await _employeeUpsertRepository.CreateEmployee(newEmployeeEntity);
        }

        public async Task DeleteEmployeeById(long id)
        {
            try
            {
                await _employeeUpsertRepository.DeleteEmployeeById(id);
            }
            catch (EntityNotFoundException)
            {
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void MergeEmployeePropertiesForUpdate(DbEmployee originalEmployee, Employee updatedEmployee)
        {
            originalEmployee.EmailAddress = updatedEmployee.Email;
            originalEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
            originalEmployee.Username = updatedEmployee.Username;
            originalEmployee.FirstName = updatedEmployee.FirstName;
            originalEmployee.LastName = updatedEmployee.LastName;
            originalEmployee.Role = (string.IsNullOrEmpty(updatedEmployee.Role)) ? originalEmployee.Role : updatedEmployee.Role;
            originalEmployee.Ssn = updatedEmployee.Ssn;
            originalEmployee.AddressLine1 = updatedEmployee.Address?.AddressLine1;
            originalEmployee.AddressLine2 = updatedEmployee.Address?.AddressLine2;
            originalEmployee.State = updatedEmployee.Address?.State;
            originalEmployee.City = updatedEmployee.Address?.City;
            originalEmployee.ZipCode = updatedEmployee.Address?.ZipCode;
        }

        private async Task ValidateEmployee(Employee employee, bool isUpdate)
        {
            ValidateEmployeeProperties(employee, isUpdate);

            await ValidateIfEmployeeIsDuplicate(employee.Id, employee.Ssn, employee.Username, employee.Email);

            if (!isUpdate)
            {
                ValidateEmployeeRole(employee.Role);
            }
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
            var isDuplicate = await _employeeRetrievalRepository.DoesEmployeeExistsBySsnOrUsernameOrEmail(ssn,
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
