using AuthenticationService.Domain.Mappers;
using AuthenticationService.Domain.Model;
using AuthenticationService.Infrastructure.EmployeeManagement;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Services
{
    public interface IEmployeeAuthHelper
    {
        Task<bool> IsAnEmployee(string username, string token);
        Task<BasicUserInfo> HandleEmployeeLogin(string username, string password, string token);
        Task HandleEmployeeLogout(long id, string role, string token);
    }

    public class EmployeeAuthHelper : IEmployeeAuthHelper
    {
        private readonly IEmployeeManagementAccessor _employeeManagementAccessor;

        public EmployeeAuthHelper(IEmployeeManagementAccessor employeeManagementAccessor)
        {
            _employeeManagementAccessor = employeeManagementAccessor;
        }

        public async Task<bool> IsAnEmployee(string username, string token)
        {
            return await _employeeManagementAccessor.CheckIfEmployee(username, token);
        }

        public async Task<BasicUserInfo> HandleEmployeeLogin(string username, string password, string token)
        {
            var employeeResponse = await _employeeManagementAccessor.Login(username, password, token);

            //employee was not found.
            if (employeeResponse == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return LoginResponseDtoMapper.MapEmployeeLoginResponseToBasicUserInfo(employeeResponse);            
        }

        public async Task HandleEmployeeLogout(long id, string role, string token)
        {
            var isEmployee = role.ToLower().Equals("employee");
            var relativeUrl = isEmployee ? $"/api/Employee/{id}/logout" : $"/api/Admin/{id}/logout";

            await _employeeManagementAccessor.Logout(id, relativeUrl, token);
        }
    }
}
