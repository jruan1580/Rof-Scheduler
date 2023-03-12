using EmployeeManagementService.Domain.Models;
using System.Threading.Tasks;

namespace EmployeeManagementService.Domain.Services
{
    public interface IEmployeeAuthService
    {
        Task<Employee> EmployeeLogIn(string username, string password);
        Task EmployeeLogout(long id);
    }
}