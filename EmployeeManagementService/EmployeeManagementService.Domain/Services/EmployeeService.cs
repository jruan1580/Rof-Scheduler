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

            return employees.Select(e => EmployeeMapper.ToCoreEmployee(e)).ToList();        
        }
    }
}
