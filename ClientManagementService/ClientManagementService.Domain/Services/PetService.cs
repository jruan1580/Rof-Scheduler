using ClientManagementService.Infrastructure.Persistence;
using ClientManagementService.Infrastructure.Persistence.Filters.Pet;
using RofShared.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetDb = ClientManagementService.Infrastructure.Persistence.Entities.Pet;
using PetToVaccineDB = ClientManagementService.Infrastructure.Persistence.Entities.PetToVaccine;

namespace ClientManagementService.Domain.Services
{
    public class PetService
    {
        protected readonly IPetRetrievalRepository _petRetrievalRepository;
        protected readonly IPetToVaccinesRepository _petToVaccinesRepository;

        public PetService(IPetRetrievalRepository petRetrievalRepository, 
            IPetToVaccinesRepository petToVaccinesRepository)
        {
            _petRetrievalRepository = petRetrievalRepository;
            _petToVaccinesRepository = petToVaccinesRepository; 
        }

        protected async Task<PetDb> GetDbPetById(long id)
        {
            var filterModel = new GetPetFilterModel<long>(GetPetFilterEnum.Id, id);

            var pet = await _petRetrievalRepository.GetPetByFilter(filterModel);

            if (pet == null)
            {
                throw new EntityNotFoundException("Pet");
            }

            return pet;
        }

        protected async Task<PetDb> GetDbPetByName(string name)
        {
            var filterModel = new GetPetFilterModel<string>(GetPetFilterEnum.Name, name);

            var pet = await _petRetrievalRepository.GetPetByFilter(filterModel);

            if (pet == null)
            {
                throw new EntityNotFoundException("Pet");
            }

            return pet;
        }

        protected async Task<List<PetToVaccineDB>> GetDbPetToVaccineByPetId(long id)
        {
            var petToVaccines = await _petToVaccinesRepository.GetPetToVaccineByPetId(id);

            if (petToVaccines == null || petToVaccines.Count == 0)
            {
                throw new EntityNotFoundException("Pet's vaccines");
            }

            return petToVaccines;
        }
    }
}
