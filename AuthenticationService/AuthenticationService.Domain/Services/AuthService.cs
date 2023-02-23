using AuthenticationService.Domain.Model;
using AuthenticationService.Infrastructure.ClientManagement;
using AuthenticationService.Infrastructure.EmployeeManagement;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Services
{
    public interface IAuthService
    {
        Task<BasicUserInfo> Login(string username, string password);

        Task Logout(long id, string role);
    }

    public class AuthService : IAuthService
    {
        private readonly IEmployeeManagementAccessor _employeeManagementAccessor;
        private readonly IClientManagementAccessor _clientManagementAccessor;
        private readonly ITokenHandler _tokenHandler;

        public AuthService(IEmployeeManagementAccessor employeeManagementAccessor,
            IClientManagementAccessor clientManagementAccessor,
            ITokenHandler tokenHandler)
        {
            _employeeManagementAccessor = employeeManagementAccessor;
            _clientManagementAccessor = clientManagementAccessor;
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
                return await HandleClientLogin(username, password, token);
            }          

            return await HandleEmployeeLogin(username, password, token);
        }

        public async Task Logout(long id, string role)
        {
            //use an internal token for internal service communication            
            var token = _tokenHandler.GenerateTokenForRole("Internal");

            if (role.ToLower().Equals("client"))
            {
                await HandleClientLogout(id, token);
                return;
            }

            await HandleEmployeeLogout(id, role, token);         
        }

        private async Task<BasicUserInfo> HandleClientLogin(string username, string password, string token)
        {
            var clientResponse = await _clientManagementAccessor.Login(username, password, token);

            if (clientResponse == null)
            {
                throw new EntityNotFoundException("Client");
            }

            return new BasicUserInfo().MapFromClientLoginResponse(clientResponse);
        }

        private async Task<BasicUserInfo> HandleEmployeeLogin(string username, string password, string token)
        {
            var employeeResponse = await _employeeManagementAccessor.Login(username, password, token);

            //employee was not found.
            if (employeeResponse == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return new BasicUserInfo().MapFromEmployeeLoginResponse(employeeResponse);
        }

        private async Task HandleClientLogout(long id, string token)
        {
            await _clientManagementAccessor.Logout(id, token);
        }

        private async Task HandleEmployeeLogout(long id, string role, string token)
        {
            var isEmployee = role.ToLower().Equals("employee");
            var relativeUrl = isEmployee ? $"/api/Employee/{id}/logout" : $"/api/Admin/{id}/logout";

            await _employeeManagementAccessor.Logout(id, relativeUrl, token);
        }
    }
}
