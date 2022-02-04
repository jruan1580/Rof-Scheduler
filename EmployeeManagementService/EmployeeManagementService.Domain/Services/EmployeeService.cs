using EmployeeManagementService.Domain.Mappers;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
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

            if(employees == null)
            {
                throw new ArgumentException("No employees found");
            }

            var employeeList = new List<Employee>();

            foreach (var employee in employees)
            {
                employeeList.Add(EmployeeMapper.DbEmployeeToCoreEmployee(employee));
            }

            return employeeList;
        }
    }
}
