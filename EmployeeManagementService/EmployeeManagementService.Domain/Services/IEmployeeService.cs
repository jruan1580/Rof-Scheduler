using EmployeeManagementService.Domain.Models;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public interface IEmployeeService
    {
        Task CreateEmployee(Employee newEmployee, string password);
        Task DeleteEmployeeById(long id);
        Task<Employee> EmployeeLogIn(string username, string password);
        Task EmployeeLogout(long id);
        Task<EmployeesWithTotalPage> GetAllEmployeesByKeyword(int page, int offset, string keyword);
        Task<Employee> GetEmployeeById(long id);
        Task<Employee> GetEmployeeByUsername(string username);
        Task ResetEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployeeActiveStatus(long id, bool active);
        Task UpdateEmployeeInformation(Employee employee);
        Task UpdatePassword(long id, string newPassword);
    }
}