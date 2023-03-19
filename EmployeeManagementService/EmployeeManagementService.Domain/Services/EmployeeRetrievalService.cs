using EmployeeManagementService.Domain.Mappers.Database;
using EmployeeManagementService.Domain.Models;
using EmployeeManagementService.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public class EmployeeRetrievalService : EmployeeService, IEmployeeRetrievalService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeRetrievalService(IEmployeeRepository employeeRepository) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
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

        public async Task<List<Employee>> GetEmployeesForDropdown()
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
