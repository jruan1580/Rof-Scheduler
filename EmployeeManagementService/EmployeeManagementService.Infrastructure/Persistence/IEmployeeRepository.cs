using EmployeeManagementService.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.Infrastructure.Persistence
{
    public interface IEmployeeRepository
    {
        Task CreateEmployee(string firstName, string lastName, string username, byte[] password, string role, bool active = true);
        Task<List<Employee>> GetAllEmployees(int page = 1, int offset = 10);
        Task UpdateEmployee(long id, string firstName, string lastName, string role, bool active);
        Task UpdatePassword(long id, byte[] newPassword);
    }
}