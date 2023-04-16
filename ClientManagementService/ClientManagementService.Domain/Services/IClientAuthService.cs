using ClientManagementService.Domain.Models;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IClientAuthService
    {
        Task<Client> ClientLogin(string username, string password);
        Task ClientLogout(long id);
    }
}