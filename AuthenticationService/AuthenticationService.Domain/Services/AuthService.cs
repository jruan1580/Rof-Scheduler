using AuthenticationService.Domain.Exceptions;
using AuthenticationService.Domain.Model;
using AuthenticationService.Infrastructure.ClientManagement;
using AuthenticationService.Infrastructure.EmployeeManagement;
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
                var clientResponse = await _clientManagementAccessor.Login(username, password, token);

                if (clientResponse == null)
                {
                    throw new NotFoundException();
                }

                return new BasicUserInfo().MapFromClientLoginResponse(clientResponse);
            }

            var employeeResponse = await _employeeManagementAccessor.Login(username, password, token);

            //employee was not found.
            if (employeeResponse == null)
            {
                throw new NotFoundException();
            }

            return new BasicUserInfo().MapFromEmployeeLoginResponse(employeeResponse);
        }

        public async Task Logout(long id, string role)
        {
            //use an internal token for internal service communication            
            var token = _tokenHandler.GenerateTokenForRole("Internal");

            if (role.ToLower().Equals("client"))
            {
                var clientResp = await _clientManagementAccessor.Logout(id, token);

                if (clientResp == null)
                {
                    throw new NotFoundException();
                }

                return;
            }

            var relativeUrl = (role.ToLower().Equals("employee")) ? $"/api/Employee/{id}/logout" : $"/api/Admin/{id}/logout";

            var resp = await _employeeManagementAccessor.Logout(id, relativeUrl, token);

            if (resp == null)
            {
                throw new NotFoundException();
            }
        }
    }
}
