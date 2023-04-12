using ClientManagementService.Domain.Models;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IClientRetrievalService
    {
        Task<ClientsWithTotalPage> GetAllClientsByKeyword(int page, int offset, string keyword);
        Task<Client> GetClientByEmail(string email);
        Task<Client> GetClientById(long id);
        Task<Client> GetClientByUsername(string username);
    }
}