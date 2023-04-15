using ClientManagementService.Domain.Models;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IClientUpsertService
    {
        Task CreateClient(Client newClient, string password);
        Task DeleteClientById(long id);
        Task ResetClientFailedLoginAttempts(long id);
        Task UpdateClientInformation(Client client);
        Task UpdatePassword(long id, string newPassword);
    }
}