using EmployeeManagementService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public interface IEmployeeRetrievalService
    {
        Task<EmployeesWithTotalPage> GetAllEmployeesByKeyword(int page, int offset, string keyword);
        Task<Employee> GetEmployeeById(long id);
        Task<Employee> GetEmployeeByUsername(string username);
        Task<List<Employee>> GetEmployeesForDropdown();
    }
}