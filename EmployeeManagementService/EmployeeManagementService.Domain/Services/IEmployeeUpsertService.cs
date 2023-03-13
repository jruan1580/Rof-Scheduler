using EmployeeManagementService.Domain.Models;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public interface IEmployeeUpsertService
    {
        Task CreateEmployee(Employee newEmployee, string password);
        Task DeleteEmployeeById(long id);
        Task ResetEmployeeFailedLoginAttempt(long id);
        Task UpdateEmployeeActiveStatus(long id, bool active);
        Task UpdateEmployeeInformation(Employee employee);
        Task UpdatePassword(long id, string newPassword);
    }
}