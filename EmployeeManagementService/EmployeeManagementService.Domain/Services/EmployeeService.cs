using EmployeeManagementService.Domain.Exceptions;
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
            var employee = await GetDbEmployeeByUsername(username);
        
            return EmployeeMapper.ToCoreEmployee(employee);
        }

        public async Task<EmployeesWithTotalPage> GetAllEmployeesByKeyword(int page, int offset, string keyword)
        {
            var result = await _employeeRepository.GetAllEmployeesByKeyword(page, offset, keyword);

            var employees = result.Item1;
            var totalPages = result.Item2;

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

            _passwordService.ValidateNewPasswordForUpdate(newPassword, employee.Password);

            employee.Password = _passwordService.EncryptPassword(newPassword);

            await _employeeRepository.UpdateEmployee(employee);
        }

        public async Task UpdateEmployeeInformation(Employee employee)
        {
            await ValidateEmployee(employee, true);

            var originalEmployee = await GetDbEmployeeById(employee.Id);

            MergeEmployeePropertiesForUpdate(originalEmployee, employee);

            await _employeeRepository.UpdateEmployee(originalEmployee);
        }

        public async Task CreateEmployee(Employee newEmployee, string password)
        {
            await ValidateEmployee(newEmployee, false);

            _passwordService.ValidatePasswordForCreate(password);             

            newEmployee.Password = _passwordService.EncryptPassword(password);

            var newEmployeeEntity = EmployeeMapper.FromCoreEmployee(newEmployee);

            await _employeeRepository.CreateEmployee(newEmployeeEntity);
        }

        public async Task<Employee> EmployeeLogIn(string username, string password)
        {
            var employee = await GetDbEmployeeByUsername(username);    
            
            await VerifyLoginPasswordAndIncrementFailedLoginAttemptsIfFail(password, employee);

            if (employee.IsLocked)
            {
                throw new EmployeeIsLockedException();
            }

            return await UpdateEmployeeStatusAndReturnEmployee(employee, true);
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
        
        private async Task VerifyLoginPasswordAndIncrementFailedLoginAttemptsIfFail(string password, DbEmployee employee)
        {
            try
            {
                _passwordService.ValidatePasswordForLogin(password, employee.Password);
            }
            catch (ArgumentException)
            {
                //password was incorrect
                await IncrementEmployeeFailedLoginAttempt(employee);

                throw;
            }
        }

        private async Task<Employee> UpdateEmployeeStatusAndReturnEmployee(DbEmployee employee, bool employeeStatus)
        {
            if (employee.Status != employeeStatus)
            {
                employee.Status = employeeStatus;

                await _employeeRepository.UpdateEmployee(employee);
            }

            return EmployeeMapper.ToCoreEmployee(employee);
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
    }
}
