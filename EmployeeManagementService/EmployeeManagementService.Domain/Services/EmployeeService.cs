using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Mappers.Database;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using Microsoft.Extensions.Configuration;
using RofShared;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public class EmployeeService : BaseEmployeeService, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        private readonly IPasswordService _passwordService;

        public EmployeeService(IEmployeeRepository employeeRepository, 
            IPasswordService passwordService, 
            IConfiguration config) : base(employeeRepository, config)
        {
            _employeeRepository = employeeRepository;

            _passwordService = passwordService;            
        }

        public async Task<Employee> GetEmployeeById(long id)
        {
            var employee = await GetDbEmployeeById(id);

            return EmployeeMapper.ToCoreEmployee(employee);
        }

        public async Task<Employee> GetEmployeeByUsername(string username)
        {
            var filterModel = new GetEmployeeFilterModel<string>(GetEmployeeFilterEnum.Usermame, username);

            var employee = await _employeeRepository.GetEmployeeByFilter(filterModel);

            if (employee == null)
            {
                return null;
            }

            return EmployeeMapper.ToCoreEmployee(employee);
        }

        public async Task<EmployeesWithTotalPage> GetAllEmployeesByKeyword(int page, int offset, string keyword)
        {
            var result = await _employeeRepository.GetAllEmployeesByKeyword(page, offset, keyword);

            var employees = result.Item1;
            var totalPages = result.Item2;

            if (employees == null || employees.Count == 0)
            {
                return new EmployeesWithTotalPage(new List<Employee>(), 0);
            }

            var coreEmployees = employees.Select(e => EmployeeMapper.ToCoreEmployee(e)).ToList();

            return new EmployeesWithTotalPage(coreEmployees, totalPages);
        }             

        public async Task ResetEmployeeFailedLoginAttempt(long id)
        {
            var employee = await GetDbEmployeeById(id);

            employee.FailedLoginAttempts = 0;
            employee.IsLocked = false;

            await _employeeRepository.UpdateEmployee(employee);
        }

        public async Task UpdateEmployeeActiveStatus(long id, bool active)
        {
            var employee = await GetDbEmployeeById(id);

            employee.Active = active;

            await _employeeRepository.UpdateEmployee(employee);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var employee = await GetDbEmployeeById(id);

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

        public async Task UpdateEmployeeInformation(Employee employee)
        {
            await ValidateEmployeeInformation(employee, true);

            var originalEmployee = await GetDbEmployeeById(employee.Id);
            
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
            await ValidateEmployeeInformation(newEmployee, false);

            if (!_passwordService.VerifyPasswordRequirements(password))
            {
                throw new ArgumentException("Password does not meet requirements");
            }            

            var encryptedPass = _passwordService.EncryptPassword(password);
            newEmployee.Password = encryptedPass;

            var newEmployeeEntity = EmployeeMapper.FromCoreEmployee(newEmployee);

            await _employeeRepository.CreateEmployee(newEmployeeEntity);
        }

        public async Task<Employee> EmployeeLogIn(string username, string password)
        {
            var employee = await GetDbEmployeeByUsername(username);

            if (employee.IsLocked)
            {
                throw new EmployeeIsLockedException();
            }

            if (!_passwordService.VerifyPasswordHash(password, employee.Password))
            {
                await IncrementEmployeeFailedLoginAttempt(employee);

                throw new ArgumentException("Incorrect password");
            }

            if (employee.Status == true)
            {
                return EmployeeMapper.ToCoreEmployee(employee);
            }

            employee.Status = true;
            await _employeeRepository.UpdateEmployee(employee);

            return EmployeeMapper.ToCoreEmployee(employee);
        }

        public async Task EmployeeLogout(long id)
        {
            var employee = await GetDbEmployeeById(id);

            if (employee.Status == false)
            {
                return;
            }

            employee.Status = false;

            await _employeeRepository.UpdateEmployee(employee);
        }        

        public async Task DeleteEmployeeById(long id)
        {
            try
            {                
                await _employeeRepository.DeleteEmployeeById(id);
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
    }
}
