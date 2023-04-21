using ClientManagementService.Domain.Models;
using System.Threading.Tasks;

namespace ClientManagementService.Domain.Services
{
    public interface IPetUpsertService
    {
        Task<long> AddPet(Pet newPet);
        Task DeletePetById(long petId);
        Task UpdatePet(Pet updatePet);
    }
}