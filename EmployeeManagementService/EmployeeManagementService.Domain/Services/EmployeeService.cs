using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Mappers.Database;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeDB = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Services
{
    public interface IEmployeeService
    {
        Task CreateEmployee(Employee newEmployee, string password);
        Task<Employee> EmployeeLogIn(string username, string password);
        Task EmployeeLogout(long id);
        Task<List<Employee>> GetAllEmployees(int page, int offset);
        Task<Employee> GetEmployeeById(long id);
        Task<Employee> GetEmployeeByUsername(string username);
        Task ResetEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployeeActiveStatus(long id, bool active);
        Task UpdateEmployeeInformation(Employee employee);
        Task UpdatePassword(long id, string newPassword);
        Task DeleteEmployeeById(long id);
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
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id));

            if (employee == null)
            {
                throw new EmployeeNotFoundException();
            }

            return EmployeeMapper.ToCoreEmployee(employee);
        }

        public async Task<Employee> GetEmployeeByUsername(string username)
        {
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<string>(GetEmployeeFilterEnum.Usermame, username));

            if (employee == null)
            {
                return null;
            }

            return EmployeeMapper.ToCoreEmployee(employee);
        }      

        public async Task ResetEmployeeFailedLoginAttempt(long id)
        {
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id));

            if (employee == null)
            {
                throw new EmployeeNotFoundException();
            }

            employee.FailedLoginAttempts = 0;
            employee.IsLocked = false;

            await _employeeRepository.UpdateEmployee(employee);
        }

        public async Task UpdateEmployeeActiveStatus(long id, bool active)
        {
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id));

            if (employee == null)
            {
                throw new EmployeeNotFoundException();
            }

            employee.Active = active;
            await _employeeRepository.UpdateEmployee(employee);
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

            if (await _employeeRepository.DoesEmployeeExistsBySsnOrUsername(employee.Ssn, employee.Username, employee.Id))
            {
                throw new ArgumentException("Employee with ssn or username exists");
            }

            var originalEmployee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, employee.Id));
            if (originalEmployee == null)
            {
                throw new EmployeeNotFoundException();
            }

            originalEmployee.EmailAddress = employee.Email;
            originalEmployee.PhoneNumber = employee.PhoneNumber;
            originalEmployee.Username = employee.Username;
            originalEmployee.FirstName = employee.FirstName;
            originalEmployee.LastName = employee.LastName;
            originalEmployee.Role = (string.IsNullOrEmpty(employee.Role)) ? originalEmployee.Role : employee.Role;
            originalEmployee.Ssn = employee.Ssn;
            originalEmployee.AddressLine1 = employee.Address?.AddressLine1;
            originalEmployee.AddressLine2 = employee.Address?.AddressLine2;
            originalEmployee.State = employee.Address?.State;
            originalEmployee.City = employee.Address?.City;
            originalEmployee.ZipCode = employee.Address?.ZipCode;
            //originalEmployee.CountryId = employee.CountryId;

            await _employeeRepository.UpdateEmployee(originalEmployee);
        }

        public async Task CreateEmployee(Employee newEmployee, string password)
        {
            var invalidErrors = newEmployee.IsValidEmployeeToCreate().ToArray();

            if (invalidErrors.Length > 0)
            {
                var errorMessage = string.Join("\n", invalidErrors);

                throw new ArgumentException(errorMessage);
            }

            if (await _employeeRepository.DoesEmployeeExistsBySsnOrUsername(newEmployee.Ssn, newEmployee.Username, newEmployee.Id))
            {
                throw new ArgumentException("Employee with ssn or username exists");
            }

            if (!_passwordService.VerifyPasswordRequirements(password))
            {
                throw new ArgumentException("Password does not meet requirements");
            }

            var roles = _roles.Split(",");

            if (!roles.Contains(newEmployee.Role))
            {
                throw new ArgumentException("Invalid role assigned");
            }

            var encryptedPass = _passwordService.EncryptPassword(password);
            newEmployee.Password = encryptedPass;

            var newEmployeeEntity = EmployeeMapper.FromCoreEmployee(newEmployee);

            await _employeeRepository.CreateEmployee(newEmployeeEntity);            
        }

        public async Task<Employee> EmployeeLogIn(string username, string password)
        {
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<string>(GetEmployeeFilterEnum.Usermame, username));

            if (employee == null)
            {
                throw new EmployeeNotFoundException();
            }

            if (employee.Status == true)
            {
                return EmployeeMapper.ToCoreEmployee(employee);
            }

            if (!_passwordService.VerifyPasswordHash(password, employee.Password))
            {
                await IncrementEmployeeFailedLoginAttempt(employee);

                throw new ArgumentException("Incorrect password");
            }

            employee.Status = true;
            await _employeeRepository.UpdateEmployee(employee);            

            return EmployeeMapper.ToCoreEmployee(employee); 
        }

        public async Task EmployeeLogout(long id)
        {
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id));

            if (employee.Status == false)
            {
                return;
            }

            employee.Status = false;

            await _employeeRepository.UpdateEmployee(employee);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id));

            if (!_passwordService.VerifyPasswordRequirements(newPassword))
            {
                throw new ArgumentException("New password does not meet all requirements.");
            }

            if (_passwordService.VerifyPasswordHash(newPassword, employee.Password))
            {
                throw new ArgumentException("New password cannot be the same as current password.");
            }

            var newEncryptedPass = _passwordService.EncryptPassword(newPassword);

            employee.Password = newEncryptedPass;

            await _employeeRepository.UpdateEmployee(employee);
        }

        public async Task DeleteEmployeeById(long id)
        {
            var employee = await _employeeRepository.GetEmployeeByFilter(new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id));
            
            if (employee == null)
            {
                return;
            }

            await _employeeRepository.DeleteEmployeeById(id);
        }

        private async Task IncrementEmployeeFailedLoginAttempt(EmployeeDB employee)
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

            await _employeeRepository.UpdateEmployee(employee);
        }
    }
}
