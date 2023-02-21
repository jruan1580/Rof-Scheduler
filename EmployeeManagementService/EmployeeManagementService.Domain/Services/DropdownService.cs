using EmployeeManagementService.Domain.Mappers.Database;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public interface IDropdownService
    {
        Task<List<Employee>> GetEmployees();
    }

    public class DropdownService : IDropdownService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public DropdownService(IEmployeeRepository employeeRepository)        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<Employee>> GetEmployees()
        {
            var employees = new List<Employee>();

            foreach (var employee in await _employeeRepository.GetEmployeesForDropdown())
            {
                employees.Add(EmployeeMapper.ToCoreEmployee(employee));
            }

            return employees;
        }
    }
}
