using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public class PetServiceDropdownService : IDropdownService<PetService>
    {
        private readonly IPetServiceRepository _petServiceRepository;

        public PetServiceDropdownService(IPetServiceRepository petServiceRepository)
        {
            _petServiceRepository = petServiceRepository;
        }

        public async Task<List<PetService>> GetDropdown()
        {
            var petServices = await _petServiceRepository.GetAllPetServicesForDropdown();

            return PetServiceMapper.ToDomainPetServices(petServices);
        }
    }
}
