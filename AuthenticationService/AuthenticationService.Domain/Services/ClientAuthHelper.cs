using AuthenticationService.Domain.Mappers;
using AuthenticationService.Domain.Model;
using AuthenticationService.Infrastructure.ClientManagement;
using RofShared.Exceptions;
using System.Threading.Tasks;

namespace AuthenticationService.Domain.Services
{
    public interface IClientAuthHelper
    {
        Task<BasicUserInfo> HandleClientLogin(string username, string password, string token);
        Task HandleClientLogout(long id, string token);
    }

    public class ClientAuthHelper : IClientAuthHelper
    {
        private readonly IClientManagementAccessor _clientManagementAccessor;

        public ClientAuthHelper(IClientManagementAccessor clientManagementAccessor)
        {
            _clientManagementAccessor = clientManagementAccessor;
        }

        public async Task<BasicUserInfo> HandleClientLogin(string username, string password, string token)
        {
            var clientResponse = await _clientManagementAccessor.Login(username, password, token);

            if (clientResponse == null)
            {
                throw new EntityNotFoundException("Client");
            }

            return LoginResponseDtoMapper.MapClientLoginResponseToBasicUserInfo(clientResponse);            
        }

        public async Task HandleClientLogout(long id, string token)
        {
            await _clientManagementAccessor.Logout(id, token);
        }
    }
}
