using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Mappers.Database;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using RofShared.Services;
using System;
using System.Threading.Tasks;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Services
{
    public class EmployeeAuthService : EmployeeService, IEmployeeAuthService
    {
        private readonly IEmployeeUpsertRepository _employeeUpsertRepository;

        private readonly IPasswordService _passwordService;

        public EmployeeAuthService(IEmployeeRetrievalRepository employeeRetrievalRepository,
            IEmployeeUpsertRepository employeeUpsertRepository,
            IPasswordService passwordService) : base(employeeRetrievalRepository)
        {
            _employeeUpsertRepository = employeeUpsertRepository;

            _passwordService = passwordService;
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

            await _employeeUpsertRepository.UpdateEmployee(employee);
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

                await _employeeUpsertRepository.UpdateEmployee(employee);
            }

            return EmployeeMapper.ToCoreEmployee(employee);
        }

        private async Task IncrementEmployeeFailedLoginAttempt(DbEmployee employee)
        {
            if (employee.IsLocked)
            {
                return;
            }

            var attempts = await _employeeUpsertRepository.IncrementEmployeeFailedLoginAttempt(employee.Id);

            if (attempts != 3)
            {
                return;
            }

            employee.IsLocked = true;
            employee.FailedLoginAttempts = attempts;

            await _employeeUpsertRepository.UpdateEmployee(employee);
        }
    }
}
