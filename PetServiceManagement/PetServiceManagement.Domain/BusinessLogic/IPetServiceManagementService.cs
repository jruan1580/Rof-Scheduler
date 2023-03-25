using PetServiceManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public interface IPetServiceManagementService
    {
        Task AddNewPetService(PetService petService);
        Task DeletePetServiceById(short id);
        Task<(List<PetService>, int)> GetPetServicesByPageAndKeyword(int page, int pageSize, string keyword = null);
        Task UpdatePetService(PetService petService);
    }
}
