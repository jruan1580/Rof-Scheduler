using AuthenticationService.Domain.Model;
using RofShared.Services;
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
        private readonly ITokenHandler _tokenHandler;
        
        private readonly IEmployeeAuthHelper _employeeAuthHelper;

        private readonly IClientAuthHelper _clientAuthHelper;

        public AuthService(ITokenHandler tokenHandler, 
            IEmployeeAuthHelper employeeAuthHelper, 
            IClientAuthHelper clientAuthHelper)
        {
            _tokenHandler = tokenHandler;

            _employeeAuthHelper = employeeAuthHelper;

            _clientAuthHelper = clientAuthHelper;
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

            var isAnEmployee = await _employeeAuthHelper.IsAnEmployee(username, token);

            //this is potentially a client
            if (!isAnEmployee)
            {
                return await _clientAuthHelper.HandleClientLogin(username, password, token);
            }          

            return await _employeeAuthHelper.HandleEmployeeLogin(username, password, token);
        }

        public async Task Logout(long id, string role)
        {
            //use an internal token for internal service communication            
            var token = _tokenHandler.GenerateTokenForRole("Internal");

            if (role.ToLower().Equals("client"))
            {
                await _clientAuthHelper.HandleClientLogout(id, token);

                return;
            }

            await _employeeAuthHelper.HandleEmployeeLogout(id, role, token);         
        }       
    }
}
