using ClientManagementService.Domain.Models;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IPetRetrievalService
    {
        Task<PetsWithTotalPage> GetAllPetsByKeyword(int page, int offset, string keyword);
        Task<Pet> GetPetById(long petId);
        Task<Pet> GetPetByName(string name);
        Task<PetsWithTotalPage> GetPetsByClientIdAndKeyword(long clientId, int page, int offset, string keyword);
    }
}