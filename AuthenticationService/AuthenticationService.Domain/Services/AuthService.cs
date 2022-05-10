using AuthenticationService.Domain.Model;
using AuthenticationService.Infrastructure.EmployeeManagement;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Services
{
    public interface IAuthService
    {
        Task<BasicUserInfo> Login(string username, string password);

        Task Logout(long id, string role, string token);
    }

    public class AuthService : IAuthService
    {
        private readonly IEmployeeManagementAccessor _employeeManagementAccessor;
        private readonly ITokenHandler _tokenHandler;

        public AuthService(IEmployeeManagementAccessor employeeManagementAccessor, ITokenHandler tokenHandler)
        {
            _employeeManagementAccessor = employeeManagementAccessor;
            _tokenHandler = tokenHandler;
        }

        /// <summary>
        /// Checks to see if it is an employee. If not, login via client.
        /// If it is employee, login via employee.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<BasicUserInfo> Login(string username, string password)
        {
            //use an internal token for internal service communication            
            var token = _tokenHandler.GenerateTokenForRole("Internal");

            var isAnEmployee = await _employeeManagementAccessor.CheckIfEmployee(username, token);

            //this is potentially a client
            if (!isAnEmployee)
            {

            }

            var employeeResponse = await _employeeManagementAccessor.Login(username, password);

            return new BasicUserInfo().MapFromEmployeeLoginResponse(employeeResponse);
        }

        public async Task Logout(long id, string role, string token)
        {
            if (role.Equals("Client"))
            {

            }

            var relativeUrl = (role.ToLower().Equals("employee")) ? $"/api/Employee/logout/{id}" : $"/api/Admin/logout/{id}";

            await _employeeManagementAccessor.Logout(id, relativeUrl, token);
        }
    }
}
