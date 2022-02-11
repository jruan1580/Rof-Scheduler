using EmployeeManagementService.Domain.Mappers;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
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
            var attempts = 0;
            var employee = await _employeeRepository.GetEmployeeById(id);

            if (employee == null)
            {
                throw new ArgumentException("Employee does not exist.");
            }

            if (employee.FailedLoginAttempts < 3)
            {
                attempts = await _employeeRepository.IncrementEmployeeFailedLoginAttempt(employee.Id);
            }

            if (attempts == 3)
            {
                await _employeeRepository.UpdateEmployeeIsLockedStatus(employee.Id, true);
            }
        }

        public async Task ResetEmployeeFailedLoginAttempt(long id)
        {
            var employee = await _employeeRepository.GetEmployeeById(id);

            if (employee == null)
            {
                throw new ArgumentException("Employee does not exist.");
            }

            await _employeeRepository.ResetEmployeeFailedLoginAttempt(employee.Id);
            await _employeeRepository.UpdateEmployeeIsLockedStatus(employee.Id, false);
        }
    }
}
